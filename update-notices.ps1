
# Ensure the script's working directory is set to the script's folder
Set-Location -Path (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet tool restore

# Iterate through all subfolders of ./src
Get-ChildItem -Path .\src -Directory | ForEach-Object {
    $currentPath = $_.FullName
    Write-Host "Processing folder: $currentPath"

    # Check if the folder contains a *.csproj file
    if (Get-ChildItem -Path $currentPath -Filter *.csproj -File -ErrorAction SilentlyContinue) {
        # Run the dotnet thirdlicense command with the current path references
        dotnet thirdlicense --project "$currentPath" --output "$currentPath\NOTICES.txt"
    } else {
        Write-Host "Skipping folder: $currentPath (no .csproj file found)"
    }
}