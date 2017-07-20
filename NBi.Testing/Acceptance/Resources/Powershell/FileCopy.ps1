#.\FileCopy.ps1 -source "C:\temp\Abo\Src" -destination "C:\temp\Abo\Dst" -filter "_20170430_"
#$source = "C:\temp\Abo\Src"
#$destination = "C:\temp\Abo\Dst" 
#$filter = [regex] "_20170430_"

param(
    [Parameter(Mandatory=$true)]
    [string]$source,
    [Parameter(Mandatory=$true)]
    [string]$destination,
    [Parameter(Mandatory=$true)]
    [string]$filter
     )

if((Test-Path -Path $destination )){
    Remove-Item -Recurse -Force $destination
}


$bin = Get-ChildItem -Recurse -Path $source | Where-Object {$_.Name -match $filter}

foreach ($item in $bin) {
    $newDir = $item.DirectoryName.replace($source,$destination)
    md $newDir -ea 0
    Copy-Item -Path $item.FullName -Destination $newDir
	Write-Host $item.FullName
}