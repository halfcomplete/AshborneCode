# Automatic Blazor to GitHub Pages deploy script

# Set paths
$blazorProjectPath = "D:\C# Projects\AshborneCode\AshborneWASM"
$publishOutputPath = "D:\C# Projects\AshborneCode\publish"
$deployRepoPath    = "D:\C# Projects\Ashborne"

# Publish Blazor project
Write-Host "`n[INFO] Publishing Blazor app..."
dotnet publish $blazorProjectPath -c Release -o $publishOutputPath /p:RunAOTCompilation=false /p:PublishTrimmed=false

if ($LASTEXITCODE -ne 0) {
    Write-Error "[ERROR] Publish failed. Aborting."
    exit $LASTEXITCODE
}

# Clean up index.html and set <base href> for GitHub Pages
$indexHtmlPath = Join-Path $publishOutputPath "wwwroot\index.html"
# Remove integrity attributes as no external assets are used
(Get-Content $indexHtmlPath) -replace ' integrity="[^"]+"', '' | Set-Content $indexHtmlPath
Write-Host "[INFO] Removed integrity attributes from index.html"
# Set <base href> to /Ashborne/ for GitHub Pages
(Get-Content $indexHtmlPath) -replace '<base href="/" />', '<base href="/Ashborne/" />' | Set-Content $indexHtmlPath
Write-Host "[INFO] Set <base href> to /Ashborne/ for GitHub Pages"

# Remove service worker files
$serviceWorkerFiles = @(
    "service-worker.js",
    "service-worker.published.js",
    "service-worker-assets.js"
)

foreach ($file in $serviceWorkerFiles) {
    $pathToRemove = Join-Path $publishOutputPath "wwwroot\$file"
    if (Test-Path $pathToRemove) {
        Remove-Item $pathToRemove -Force
        Write-Host "[INFO] Removed: $file"
    }
}

# Copy published files to GitHub Pages repo
$source = Join-Path $publishOutputPath "wwwroot"
$destination = $deployRepoPath

Write-Host "`n[INFO] Copying published wwwroot to deploy repo:"
Write-Host "$source -> $destination"

if (!(Test-Path $source)) {
    Write-Error "[ERROR] wwwroot not found at $source. Did publish succeed?"
    exit 1
}

if ($destination -ne "D:\C# Projects\Ashborne") {
    Write-Error "[ERROR] 'destination' set to $destination, instead of 'D:\C# Projects\Ashborne'. Exiting for safety"
    exit 1
}


# Delete all files except .git
Get-ChildItem -Path $destination -Recurse -Force |
    Where-Object { $_.FullName -notlike "$destination\.git*" } |
    Remove-Item -Force -Recurse

# Copy new files in
Copy-Item -Path "$source\*" -Destination $destination -Recurse -Force

# Git commit and push
Write-Host "`n[INFO] Committing and pushing to GitHub Pages..."

Set-Location $deployRepoPath
git add .

$commitMessage = "Deploy from publish on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
git commit -m "$commitMessage"
git push

if ($LASTEXITCODE -ne 0) {
    Write-Error "[ERROR] Git push failed. Check remote connection or conflicts."
    exit $LASTEXITCODE
}

Write-Host "`n[SUCCESS] Deployment complete! Visit: https://halfcomplete.github.io/Ashborne/"


# Revert <base href> in SOURCE index.html to local (/)
$sourceIndexHtmlPath = Join-Path $blazorProjectPath "wwwroot\index.html"
if (Test-Path $sourceIndexHtmlPath) {
    (Get-Content $sourceIndexHtmlPath) -replace '<base href="/Ashborne/" />', '<base href="/" />' | Set-Content $sourceIndexHtmlPath
    Write-Host "[INFO] Reverted <base href> in SOURCE index.html to / for local development."
} else {
    Write-Warning "[WARNING] Source index.html not found at $sourceIndexHtmlPath."
}

# Return to AshborneCode root
Set-Location (Split-Path $blazorProjectPath -Parent)
