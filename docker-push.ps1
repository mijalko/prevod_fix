# Usage: .\docker-push.ps1 -Username your_dockerhub_username [-Tag latest]
param(
    [Parameter(Mandatory=$true)]
    [string]$Username,

    [string]$Tag = "latest"
)

$Image = "$Username/subtitle-fixer:$Tag"

Write-Host "Building image: $Image" -ForegroundColor Cyan
docker build -t $Image -f Dockerfile .

if ($LASTEXITCODE -ne 0) { Write-Error "Build failed."; exit 1 }

Write-Host "Pushing $Image to Docker Hub..." -ForegroundColor Cyan
docker push $Image

if ($LASTEXITCODE -eq 0) {
    Write-Host "Done! Image available at: https://hub.docker.com/r/$Username/subtitle-fixer" -ForegroundColor Green
} else {
    Write-Error "Push failed. Make sure you are logged in: docker login"
}
