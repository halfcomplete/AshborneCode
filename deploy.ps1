# === deploy.ps1 ===
# Eric's automatic Blazor to GitHub Pages deploy script

# === Paths ===
$blazorProjectPath = "D:\C# Projects\AshborneCode\AshborneWASM"
$publishOutputPath = "D:\C# Projects\AshborneCode\publish"
$deployRepoPath    = "D:\C# Projects\Ashborne"

# === Step 1: Publish Blazor project WITHOUT service worker ===
Write-Host "`n[INFO] Publishing Blazor app (no service worker)..."
dotnet publish $blazorProjectPath -c Release -o $publishOutputPath /p:RunAOTCompilation=false /p:PublishTrimmed=false

if ($LASTEXITCODE -ne 0) {
    Write-Error "[ERROR] Publish failed. Aborting."
    exit $LASTEXITCODE
}

# === Step 2: Clean up index.html ===
$indexHtmlPath = Join-Path $publishOutputPath "wwwroot\index.html"
(Get-Content $indexHtmlPath) -replace ' integrity="[^"]+"', '' | Set-Content $indexHtmlPath
Write-Host "[INFO] Removed integrity attributes from index.html"

# === Remove Native AOT artifacts ===
#$nativeArtifacts = @(
#    "_framework/dotnet.native.wasm",
#    "_framework/dotnet.native.js"
#)

#foreach ($artifact in $nativeArtifacts) {
#    $nativePath = Join-Path $publishOutputPath "wwwroot\$artifact"
#    if (Test-Path $nativePath) {
#        Remove-Item $nativePath -Force
#        Write-Host "[INFO] Removed Native AOT artifact: $artifact"
#    }
#}

# === Remove pre-compressed AOT files ===
#$compressedNativeFiles = @(
#    "_framework/dotnet.native.br",
#    "_framework/dotnet.native.js.br",
#    "_framework/dotnet.native.js.gz",
#    "_framework/dotnet.native.gz",
#    "_framework/dotnet.native.wasm.br",
#    "_framework/dotnet.native.wasm.gz"
#)

#foreach ($file in $compressedNativeFiles) {
#    $path = Join-Path $publishOutputPath "wwwroot\$file"
#    if (Test-Path $path) {
#        Remove-Item $path -Force
#        Write-Host "[INFO] Removed compressed native file: $file"
#    }
#}


# # === Step 3: Clear SHA256 hashes from blazor.boot.json ===
# $bootJsonPath = Join-Path $publishOutputPath "wwwroot\_framework\blazor.boot.json"
# Write-Host "[DEBUG] blazor.boot.json path: $bootJsonPath"

# if (Test-Path $bootJsonPath) {
#    (Get-Content $bootJsonPath -Raw) -replace '\"sha256-[^\"]+\"', '""' | Set-Content $bootJsonPath -Encoding UTF8
#    Write-Host "[INFO] Cleared all SHA256 hashes in blazor.boot.json"
# } else {
#    Write-Warning "[WARNING] blazor.boot.json not found at expected path."
# }

# === Step 4: Remove service worker files ===
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

# === Step 5: Copy published files to GitHub Pages repo ===
$source = Join-Path $publishOutputPath "wwwroot"
$destination = $deployRepoPath

Write-Host "`n[INFO] Copying published wwwroot to deploy repo:"
Write-Host "$source -> $destination"

if (!(Test-Path $source)) {
    Write-Error "[ERROR] wwwroot not found at $source. Did publish succeed?"
    exit 1
}

# Delete all files except .git
Get-ChildItem -Path $destination -Recurse -Force |
    Where-Object { $_.FullName -notlike "$destination\.git*" } |
    Remove-Item -Force -Recurse

# Copy new files in
Copy-Item -Path "$source\*" -Destination $destination -Recurse -Force

# === Step 6: Git commit and push ===
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

# Return to AshborneCode root
Set-Location (Split-Path $blazorProjectPath -Parent)
