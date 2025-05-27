Push-Location $PSScriptRoot

$secretsPath = Join-Path $PSScriptRoot "..\secrets.json"
$secretsJson = Get-Content $secretsPath -Raw | ConvertFrom-Json

function Flatten-Json($json, $prefix = "") {
    $result = @{}

    foreach ($property in $json.PSObject.Properties) {
        $key = if ($prefix) { "${prefix}:$($property.Name)" } else { $property.Name }

        if ($property.Value -is [System.Management.Automation.PSCustomObject]) {
            $result += Flatten-Json $property.Value $key
        } else {
            $result[$key] = $property.Value
        }
    }

    return $result
}

$flatSecrets = Flatten-Json $secretsJson

foreach ($kv in $flatSecrets.GetEnumerator()) {
    Write-Host "Setting $($kv.Key)"
    dotnet user-secrets set "$($kv.Key)" "$($kv.Value)" --project "$PSScriptRoot"
}

Pop-Location