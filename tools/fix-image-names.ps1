# Renomeia arquivos em Resources\Images substituindo '-' por '_' e garantindo primeiro/último caractere letra
Get-ChildItem -Path .\src\desktop\Resources\Images -Recurse -File -Include *.png,*.jpg,*.svg,*.jpeg |
ForEach-Object {
    $orig = $_.FullName
    $name = $_.BaseName.ToLower().Replace('-','_')
    # garantir que inicie e termine com letra (ajusta somente se necessário)
    if (-not ($name[0] -match '[a-z]')) { $name = "a$name" }
    if (-not ($name[-1] -match '[a-z]')) { $name = "$name" } # evite adicionar se terminar com letra ok
    $newFile = Join-Path $_.DirectoryName ($name + $_.Extension.ToLower())
    if ($orig -ne $newFile) {
        Write-Host "Renomeando: $orig -> $newFile"
        Rename-Item -LiteralPath $orig -NewName (Split-Path $newFile -Leaf)
    }
}