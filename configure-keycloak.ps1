$KEYCLOAK_URL = "http://localhost:8080"
$REALM = "GameNest"

# 1. Get admin token
$token = (Invoke-RestMethod -Uri "$KEYCLOAK_URL/realms/master/protocol/openid-connect/token" `
    -Method Post -ContentType "application/x-www-form-urlencoded" `
    -Body "username=admin&password=admin&grant_type=password&client_id=admin-cli").access_token

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# 2. Download all scopes and roles
$scopes = Invoke-RestMethod "$KEYCLOAK_URL/admin/realms/$REALM/client-scopes" -Headers $headers
$roles  = Invoke-RestMethod "$KEYCLOAK_URL/admin/realms/$REALM/roles" -Headers $headers

# 3. Extract role's ID
$adminId   = ($roles | ? name -eq "admin").id
$managerId = ($roles | ? name -eq "manager").id
$userId    = ($roles | ? name -eq "user").id

# 4. Get ID scopes
function GetScopeId($name) {
    return ($scopes | ? name -eq $name).id
}

$scopeNames = @(
    "catalog:write", "catalog:delete",
    "orders:read", "orders:create", "orders:update", "orders:delete",
    "payments:read", "payments:create", "payments:update", "payments:delete",
    "reviews:write", "reviews:update", "reviews:delete"
)

$scopeIds = @{}
foreach ($s in $scopeNames) { $scopeIds[$s] = GetScopeId $s }

# 5. JSON
$adminJson   = "[{`"id`":`"$adminId`",`"name`":`"admin`"}]"
$managerJson = "[{`"id`":`"$managerId`",`"name`":`"manager`"}]"
$userJson    = "[{`"id`":`"$userId`",`"name`":`"user`"}]"

Write-Host "Configuring role-to-scope mappings..." -ForegroundColor Cyan

# 6. Admin
$adminScopes = @(
    "catalog:write", "catalog:delete",
    "orders:read", "orders:create", "orders:update", "orders:delete",
    "payments:read", "payments:create", "payments:update", "payments:delete",
    "reviews:write", "reviews:update", "reviews:delete"
)

foreach ($s in $adminScopes) {
    $id = $scopeIds[$s]
    if ($id) {
        Invoke-RestMethod "$KEYCLOAK_URL/admin/realms/$REALM/client-scopes/$id/scope-mappings/realm" `
            -Method Post -Headers $headers -Body $adminJson | Out-Null
    }
}

# 7. Manager
$managerScopes = @(
    "catalog:write",
    "orders:read", "orders:create", "orders:update",
    "payments:read", "payments:create", "payments:update",
    "reviews:write", "reviews:update", "reviews:delete"
)

foreach ($s in $managerScopes) {
    $id = $scopeIds[$s]
    if ($id) {
        Invoke-RestMethod "$KEYCLOAK_URL/admin/realms/$REALM/client-scopes/$id/scope-mappings/realm" `
            -Method Post -Headers $headers -Body $managerJson | Out-Null
    }
}

# 8. User
$userScopes = @(
    "orders:read", "orders:create",
    "payments:read", "payments:create",
    "reviews:write", "reviews:update", "reviews:delete"
)

foreach ($s in $userScopes) {
    $id = $scopeIds[$s]
    if ($id) {
        Invoke-RestMethod "$KEYCLOAK_URL/admin/realms/$REALM/client-scopes/$id/scope-mappings/realm" `
            -Method Post -Headers $headers -Body $userJson | Out-Null
    }
}

Write-Host "Done. Role-to-scope mappings applied." -ForegroundColor Green