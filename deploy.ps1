Write-Host "Deploying Keycloak..." -ForegroundColor Cyan

cd infrastructure
pulumi up --yes

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed!" -ForegroundColor Red
    exit 1
}

cd ..
Start-Sleep -Seconds 5

.\configure-keycloak.ps1

Write-Host "`nDone! Test at https://localhost:7048/swagger" -ForegroundColor Green