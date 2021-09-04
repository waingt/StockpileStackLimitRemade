$textEngine = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\Common7\IDE\TextTransform.exe"
& $textEngine .\About\About.tt
& $textEngine .\About\ModSync.tt
& $textEngine .\workshop.tt
$committed = ($null -eq (git status --porcelain))
if (-not $committed) { 
    Write-Output 'not committed yet.'
    return 
}
Remove-Item .\.vs -Recurse -Force
Remove-Item .\Source\obj -Recurse -Force
$filepath = Resolve-Path .\workshop.vdf
steamcmd.exe +login 1766744431 +workshop_build_item $filepath +quit
$version = Get-Content .\About\version.txt
git tag "v$version"
git push origin
git push origin --tags