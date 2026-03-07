#!/usr/bin/env pwsh

param(
	[string]$KubeContext = 'tst-services-cluster-uks-aks',
	[string]$Namespace = 'dev-documentsservice',
	[string]$ServiceName = 'svc-documents',
	[int]$LocalPort = 8999,
	[int]$RemotePort = 80
)

$ErrorActionPreference = 'Stop'

try {
	$serviceTarget = if ($ServiceName -like 'svc/*') { $ServiceName } else { "svc/$ServiceName" }

	Write-Host "Starting port-forward: $serviceTarget -> localhost:$LocalPort"
	Write-Host 'Press Ctrl+C to stop port-forwards.'

	$kubectlArgs = @(
		'--context',
		$KubeContext,
		'-n',
		$Namespace,
		'port-forward',
		$serviceTarget,
		"${LocalPort}:${RemotePort}"
	)

	& kubectl @kubectlArgs
	$exitCode = $LASTEXITCODE
	if ($exitCode -ne 0) {
		Write-Error 'kubectl port-forward failed. Ensure you are connected to the Azure VPN and can reach the cluster.'
		exit $exitCode
	}
}
finally {
}
