# === deploy.ps1 ===
# Eric's automatic Blazor to GitHub Pages deploy script
# Paths
$blazorProjectPath = "D:\C# Projects\AshborneCode\AshborneWASM"
$publishOutputPath = "D:\C# Projects\AshborneCode\publish"
$deployRepoPath    = "D:\C# Projects\Ashborne"

# Step 1: Publish Blazor project WITHOUT service worker
Write-Host ""
Write-Host "[INFO] Publishing Blazor app (no service worker)..."
dotnet publish $blazorProjectPath -c Debug -o $publishOutputPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "[ERROR] Publish failed. Aborting."
    exit $LASTEXITCODE
}

# Step 2: Remove service worker files from published output
$serviceWorkerFiles = @(
    "service-worker.js",
    "service-worker.published.js",
    "service-worker-assets.js",
    "manifest.webmanifest"
)

foreach ($file in $serviceWorkerFiles) {
    $pathToRemove = Join-Path $publishOutputPath "wwwroot\$file"
    if (Test-Path $pathToRemove) {
        Remove-Item $pathToRemove -Force
        Write-Host "[INFO] Removed: $file"
    }
}

# Step 3: Copy wwwroot contents from publish to GitHub Pages folder
$source = Join-Path $publishOutputPath "wwwroot"
$destination = $deployRepoPath

Write-Host ""
Write-Host "[INFO] Copying published wwwroot to deploy repo:"
Write-Host "$source -> $destination"

if (!(Test-Path $source)) {
    Write-Error "[ERROR] wwwroot not found at $source. Did publish succeed?"
    exit 1
}

# Clear old files but exclude .git folder
Get-ChildItem -Path $destination -Recurse -Force |
    Where-Object { $_.FullName -notlike "$destination\.git*" } |
    Remove-Item -Force -Recurse

# Copy fresh contents
Copy-Item -Path "$source\*" -Destination $destination -Recurse -Force

# Step 4: Git commit and push
Write-Host ""
Write-Host "[INFO] Committing and pushing to GitHub Pages..."

Set-Location $deployRepoPath
git add .

$commitMessage = "Deploy from publish on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
git commit -m "$commitMessage"
git push

Write-Host ""
Write-Host "[SUCCESS] Deployment complete! Visit: https://halfcomplete.github.io/Ashborne/"

cd ..
cd AshborneCode
