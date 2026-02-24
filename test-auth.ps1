# Test authentication without token
Write-Host "Testing ProductService without authentication..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001/api/product" -Method GET -UseBasicParsing -ErrorAction Stop
    Write-Host "ERROR: Request succeeded without authentication!"
} catch {
    Write-Host "SUCCESS: Authentication is working - request failed as expected"
    Write-Host "Status: $($_.Exception.Response.StatusCode)"
}

# Register a user
Write-Host "`nRegistering a test user..."
$registerBody = @{
    username = "testuser"
    passwordHash = "testpass"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json" -UseBasicParsing
    Write-Host "User registered: $($registerResponse.Content)"
} catch {
    Write-Host "Register failed: $($_.Exception.Message)"
}

# Login to get token
Write-Host "`nLogging in to get token..."
$loginBody = @{
    username = "testuser"
    password = "testpass"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json" -UseBasicParsing
    $loginData = $loginResponse.Content | ConvertFrom-Json
    $token = $loginData.token
    Write-Host "Login successful, token received"
    
    # Test ProductService with token
    Write-Host "`nTesting ProductService with authentication..."
    $headers = @{
        "Authorization" = "Bearer $token"
    }
    
    $authResponse = Invoke-WebRequest -Uri "http://localhost:5001/api/product" -Method GET -Headers $headers -UseBasicParsing
    Write-Host "SUCCESS: Authenticated request worked!"
    Write-Host "Response: $($authResponse.Content)"
    
} catch {
    Write-Host "Login or authenticated request failed: $($_.Exception.Message)"
}
