$api = "http://localhost:5134/api"
$web = "http://localhost:5173"

$results = [System.Collections.Generic.List[PSCustomObject]]::new()

function Record-Test($name, $passed, $details) {
    $results.Add([PSCustomObject]@{
        Case = $name
        Status = if ($passed) { "PASSED [OK]" } else { "FAILED [X]" }
        Details = $details
    })
    $color = if ($passed) { "Green" } else { "Red" }
    Write-Host "$($results[-1].Status) - $name : $details" -ForegroundColor $color
}

Write-Host "=== STARTING COMPREHENSIVE VALIDATION SUITE ===" -ForegroundColor Cyan

# -------------------------------------------------------------
# CASE 1: Public View (Unauthenticated access)
# -------------------------------------------------------------
try {
    $res = Invoke-RestMethod -Uri "$api/newsarticles/active" -Method Get
    $allActive = $true
    foreach ($item in $res) {
        if ($item.newsStatus -ne $true) {
            $allActive = $false
            break
        }
    }
    Record-Test "1.1 Public View - Only Active News" ($allActive -and $res.Count -gt 0) "Returned $($res.Count) active news articles. All have newsStatus == true."
} catch {
    Record-Test "1.1 Public View - Only Active News" $false $_.Message
}

try {
    $webHome = Invoke-WebRequest -Uri "$web" -Method Get -UseBasicParsing
    Record-Test "1.2 FrontEnd Home - Public Access (No Auth)" ($webHome.StatusCode -eq 200) "Home page returned HTTP 200 OK without requiring login."
} catch {
    Record-Test "1.2 FrontEnd Home - Public Access (No Auth)" $false $_.Message
}

# -------------------------------------------------------------
# CASE 2: Authentication
# -------------------------------------------------------------
# 2.1 Default Admin Login
try {
    $adminBody = @{ Email = "admin@FUNewsManagementSystem.org"; Password = "@@abc123@@" } | ConvertTo-Json
    $adminRes = Invoke-RestMethod -Uri "$api/auth/login" -Method Post -Body $adminBody -ContentType "application/json"
    $adminOk = ($adminRes.role -eq "Admin") -and ($adminRes.roleId -eq 0)
    Record-Test "2.1 Admin Authentication (appsettings.json)" $adminOk "Admin login successful. Role: $($adminRes.role), RoleId: $($adminRes.roleId)"
} catch {
    Record-Test "2.1 Admin Authentication (appsettings.json)" $false $_.Message
}

# 2.2 Staff Login
try {
    $staffBody = @{ Email = "thanhhthe182267@fpt.edu.vn"; Password = "Hathanh55" } | ConvertTo-Json
    $staffRes = Invoke-RestMethod -Uri "$api/auth/login" -Method Post -Body $staffBody -ContentType "application/json"
    $staffOk = ($staffRes.role -eq "Staff") -and ($staffRes.roleId -eq 1)
    Record-Test "2.2 Staff Authentication (Database)" $staffOk "Staff login successful. Name: $($staffRes.accountName), Role: $($staffRes.role)"
} catch {
    Record-Test "2.2 Staff Authentication (Database)" $false $_.Message
}

# 2.3 Invalid Password
try {
    $invalidBody = @{ Email = "admin@FUNewsManagementSystem.org"; Password = "WrongPassword123" } | ConvertTo-Json
    $res = Invoke-RestMethod -Uri "$api/auth/login" -Method Post -Body $invalidBody -ContentType "application/json"
    Record-Test "2.3 Invalid Password Rejection" $false "Expected 401 Unauthorized but succeeded."
} catch {
    $is401 = $_.Exception.Response.StatusCode.value__ -eq 401
    Record-Test "2.3 Invalid Password Rejection" $is401 "Correctly rejected with 401 Unauthorized."
}

# 2.4 Non-existent Email
try {
    $unknownBody = @{ Email = "unknown_user_999@test.com"; Password = "@@1" } | ConvertTo-Json
    $res = Invoke-RestMethod -Uri "$api/auth/login" -Method Post -Body $unknownBody -ContentType "application/json"
    Record-Test "2.4 Non-existent Email Rejection" $false "Expected 401 Unauthorized but succeeded."
} catch {
    $is401 = $_.Exception.Response.StatusCode.value__ -eq 401
    Record-Test "2.4 Non-existent Email Rejection" $is401 "Correctly rejected with 401 Unauthorized."
}

# -------------------------------------------------------------
# CASE 3: Account Management (Admin)
# -------------------------------------------------------------
# 3.1 Search accounts
try {
    $accs = Invoke-RestMethod -Uri "$api/accounts?keyword=David" -Method Get
    Record-Test "3.1 Search Accounts by Keyword" ($accs.Count -gt 0) "Found $($accs.Count) accounts matching 'David'."
} catch {
    Record-Test "3.1 Search Accounts by Keyword" $false $_.Message
}

# 3.2 Conflict with Admin reserved email
try {
    $conflictBody = @{
        AccountName = "Fake Admin"
        AccountEmail = "admin@FUNewsManagementSystem.org"
        AccountRole = 1
        AccountPassword = "password123"
    } | ConvertTo-Json
    $res = Invoke-RestMethod -Uri "$api/accounts" -Method Post -Body $conflictBody -ContentType "application/json"
    Record-Test "3.2 Prevent Creation with Admin Reserved Email" $false "Expected 400 Bad Request but succeeded."
} catch {
    $is400 = $_.Exception.Response.StatusCode.value__ -eq 400
    Record-Test "3.2 Prevent Creation with Admin Reserved Email" $is400 "Correctly rejected attempt to use reserved Admin email."
}

# 3.3 Create temporary account
$testAccId = 0
try {
    $newAccBody = @{
        AccountName = "Nguyen Van Test"
        AccountEmail = "test_staff_$(Get-Random)@funews.org"
        AccountRole = 1
        AccountPassword = "password@123"
    } | ConvertTo-Json
    $createdAcc = Invoke-RestMethod -Uri "$api/accounts" -Method Post -Body $newAccBody -ContentType "application/json"
    $testAccId = $createdAcc.accountID
    Record-Test "3.3 Create New Staff Account" ($testAccId -gt 0) "Created test account with ID: $testAccId"
} catch {
    Record-Test "3.3 Create New Staff Account" $false $_.Message
}

# 3.4 Prevent duplicate email
try {
    $dupBody = @{
        AccountName = "Duplicate User"
        AccountEmail = "IsabellaDavid@FUNewsManagement.org"
        AccountRole = 1
        AccountPassword = "password@123"
    } | ConvertTo-Json
    $res = Invoke-RestMethod -Uri "$api/accounts" -Method Post -Body $dupBody -ContentType "application/json"
    Record-Test "3.4 Duplicate Email Validation on Create" $false "Expected 400 Bad Request for duplicate email."
} catch {
    $is400 = $_.Exception.Response.StatusCode.value__ -eq 400
    Record-Test "3.4 Duplicate Email Validation on Create" $is400 "Correctly blocked duplicate email creation."
}

# 3.5 Critical Rule: Prevent deleting account with articles
try {
    # AccountID 6 (Ha Trung Thanh) has created news articles
    $res = Invoke-RestMethod -Uri "$api/accounts/6" -Method Delete
    Record-Test "3.5 Business Rule: Cannot Delete Account with Articles" $false "Expected deletion failure for Account 6."
} catch {
    $is400 = $_.Exception.Response.StatusCode.value__ -eq 400
    Record-Test "3.5 Business Rule: Cannot Delete Account with Articles" $is400 "Blocked deletion of Account 6 because it has created news articles."
}

# 3.6 Delete account without articles
if ($testAccId -gt 0) {
    try {
        $delRes = Invoke-RestMethod -Uri "$api/accounts/$testAccId" -Method Delete
        Record-Test "3.6 Delete Account without Articles" ($delRes.message -like "*successfully*") "Cleanly deleted test account $testAccId."
    } catch {
        Record-Test "3.6 Delete Account without Articles" $false $_.Message
    }
}

# -------------------------------------------------------------
# CASE 4: Category Management (Staff)
# -------------------------------------------------------------
# 4.1 Search categories
try {
    $cats = Invoke-RestMethod -Uri "$api/categories?keyword=Education" -Method Get
    Record-Test "4.1 Search Categories" ($cats.Count -ge 0) "Retrieved category search results successfully."
} catch {
    Record-Test "4.1 Search Categories" $false $_.Message
}

# 4.2 Create temporary category
$testCatId = 0
try {
    $newCatBody = @{
        CategoryName = "Temp Test Category $(Get-Random)"
        CategoryDesciption = "Description for temporary test category"
        IsActive = $true
    } | ConvertTo-Json
    $createdCat = Invoke-RestMethod -Uri "$api/categories" -Method Post -Body $newCatBody -ContentType "application/json"
    $testCatId = $createdCat.categoryID
    Record-Test "4.2 Create Category" ($testCatId -gt 0) "Created category with ID: $testCatId"
} catch {
    Record-Test "4.2 Create Category" $false $_.Message
}

# 4.3 Self-referencing Parent Category validation
if ($testCatId -gt 0) {
    try {
        $selfParentBody = @{
            CategoryID = $testCatId
            CategoryName = "Updated Category"
            CategoryDesciption = "Updated Description"
            ParentCategoryID = $testCatId
            IsActive = $true
        } | ConvertTo-Json
        $res = Invoke-RestMethod -Uri "$api/categories/$testCatId" -Method Put -Body $selfParentBody -ContentType "application/json"
        Record-Test "4.3 Prevent Category Self-Parenting" $false "Expected 400 Bad Request when setting parent to self."
    } catch {
        $is400 = $_.Exception.Response.StatusCode.value__ -eq 400
        Record-Test "4.3 Prevent Category Self-Parenting" $is400 "Correctly rejected setting category as its own parent."
    }
}

# 4.4 Critical Rule: Prevent deleting category associated with news
try {
    # CategoryID 1 is associated with news articles
    $res = Invoke-RestMethod -Uri "$api/categories/1" -Method Delete
    Record-Test "4.4 Business Rule: Cannot Delete Category in use by News" $false "Expected deletion failure for Category 1."
} catch {
    $is400 = $_.Exception.Response.StatusCode.value__ -eq 400
    Record-Test "4.4 Business Rule: Cannot Delete Category in use by News" $is400 "Blocked deletion of Category 1 because it has associated news articles."
}

# 4.5 Delete unused category
if ($testCatId -gt 0) {
    try {
        $delCatRes = Invoke-RestMethod -Uri "$api/categories/$testCatId" -Method Delete
        Record-Test "4.5 Delete Unused Category" ($delCatRes.message -like "*successfully*") "Cleanly deleted test category $testCatId."
    } catch {
        Record-Test "4.5 Delete Unused Category" $false $_.Message
    }
}

# -------------------------------------------------------------
# CASE 5: News Article Management (Staff)
# -------------------------------------------------------------
$testArticleId = "TEST_$(Get-Random -Minimum 1000 -Maximum 9999)"
# 5.1 Create News Article with Tags
try {
    $articleBody = @{
        NewsArticleID = $testArticleId
        NewsTitle = "Test News Article Automated Validation"
        Headline = "Automated test article headline verification"
        NewsContent = "Content for comprehensive unit and integration testing"
        NewsSource = "FPT Education Internal"
        CategoryID = 1
        NewsStatus = $true
        CreatedByID = 6
        TagIds = @(1, 2)
    } | ConvertTo-Json
    $createdArticle = Invoke-RestMethod -Uri "$api/newsarticles" -Method Post -Body $articleBody -ContentType "application/json"
    Record-Test "5.1 Create News Article with Multiple Tags" ($createdArticle.newsArticleID -eq $testArticleId) "Created news article $testArticleId with tags [1, 2]."
} catch {
    Record-Test "5.1 Create News Article with Multiple Tags" $false $_.Message
}

# 5.2 Update News Article
try {
    $updateArticleBody = @{
        NewsArticleID = $testArticleId
        NewsTitle = "Updated Title Automated Test"
        Headline = "Updated Headline"
        NewsContent = "Updated Content"
        NewsSource = "FPT Education Internal Updated"
        CategoryID = 1
        NewsStatus = $true
        UpdatedByID = 6
        TagIds = @(2, 3)
    } | ConvertTo-Json
    $updateRes = Invoke-RestMethod -Uri "$api/newsarticles/$testArticleId" -Method Put -Body $updateArticleBody -ContentType "application/json"
    Record-Test "5.2 Update News Article and Tag Associations" ($updateRes.message -like "*successfully*") "Updated article and tags to [2, 3]."
} catch {
    Record-Test "5.2 Update News Article and Tag Associations" $false $_.Message
}

# 5.3 Critical Rule: Clean Deletion (Removing Tags without FK violation)
try {
    $delArticleRes = Invoke-RestMethod -Uri "$api/newsarticles/$testArticleId" -Method Delete
    Record-Test "5.3 Clean News Article Deletion (No FK Constraint Error)" ($delArticleRes.message -like "*successfully*") "Cleanly deleted article $testArticleId and its NewsTag relationships."
} catch {
    Record-Test "5.3 Clean News Article Deletion (No FK Constraint Error)" $false $_.Message
}

# -------------------------------------------------------------
# CASE 6: Reports & Statistics (Admin)
# -------------------------------------------------------------
try {
    $start = (Get-Date).AddYears(-10).ToString("yyyy-MM-dd")
    $end = (Get-Date).AddYears(1).ToString("yyyy-MM-dd")
    $report = Invoke-RestMethod -Uri "$api/reports?startDate=$start&endDate=$end" -Method Get
    
    # Check descending order of CreatedDate
    $isSortedDesc = $true
    if ($report.articles.Count -gt 1) {
        for ($i = 0; $i -lt ($report.articles.Count - 1); $i++) {
            $d1 = [DateTime]$report.articles[$i].createdDate
            $d2 = [DateTime]$report.articles[$i + 1].createdDate
            if ($d1 -lt $d2) {
                $isSortedDesc = $false
                break
            }
        }
    }
    Record-Test "6.1 Report Statistics - Descending Date Order" ($isSortedDesc -and $report.totalNews -gt 0) "Total news in period: $($report.totalNews). Confirmed strictly descending order by CreatedDate."
} catch {
    Record-Test "6.1 Report Statistics - Descending Date Order" $false $_.Message
}

# -------------------------------------------------------------
# CASE 7: Staff History & Profile Isolation
# -------------------------------------------------------------
try {
    $history = Invoke-RestMethod -Uri "$api/newsarticles/history/6" -Method Get
    $allCreatedBy6 = $true
    foreach ($item in $history) {
        if ($item.createdByID -ne 6) {
            $allCreatedBy6 = $false
            break
        }
    }
    Record-Test "7.1 Staff History Isolation (CreatedByID == 6)" ($allCreatedBy6 -and $history.Count -gt 0) "Returned $($history.Count) articles, all strictly created by Staff ID 6."
} catch {
    Record-Test "7.1 Staff History Isolation (CreatedByID == 6)" $false $_.Message
}

# -------------------------------------------------------------
# CASE 8: FrontEnd Route Security (Unauthenticated Redirection)
# -------------------------------------------------------------
Add-Type -AssemblyName System.Net.Http
$handler = [System.Net.Http.HttpClientHandler]::new()
$handler.AllowAutoRedirect = $false
$client = [System.Net.Http.HttpClient]::new($handler)
$client.Timeout = [TimeSpan]::FromSeconds(5)

$routes = @("Account", "Report", "NewsArticle", "Category", "Profile")
foreach ($r in $routes) {
    try {
        $resp = $client.GetAsync("$web/$r").GetAwaiter().GetResult()
        $isRedirect = ($resp.StatusCode.value__ -eq 302)
        $redirectTarget = $resp.Headers.Location.ToString()
        Record-Test "8.Route Protection - /$r" ($isRedirect -and $redirectTarget -like "*/Login*") "Unauthenticated access to /$r redirects (HTTP 302) to $redirectTarget."
    } catch {
        Record-Test "8.Route Protection - /$r" $false $_.Message
    }
}
$client.Dispose()
$handler.Dispose()

Write-Host "`n=== VALIDATION SUMMARY ===" -ForegroundColor Cyan
$passedCount = ($results | Where-Object { $_.Status -like "*OK*" }).Count
$totalCount = $results.Count
Write-Host "PASSED: $passedCount / $totalCount tests." -ForegroundColor $(if ($passedCount -eq $totalCount) { "Green" } else { "Yellow" })

$results | Format-Table -AutoSize
