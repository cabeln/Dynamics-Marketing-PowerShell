# MDMPowerShell
PowerShell command lets for Microsoft Dynamics Marketing

This project showcases how to use the API and the OData endpoint of a Microsoft Dynamics Marketing instance through Microsoft PowerShell. Two modules a compiled from the solution which implement a set of cmdlets that cover the full SDK and allow to use "Find" methods in order to search sets of entities including terms like  filter, sort, skip, top, expand.

1. Install the command let mosules on your local machine
2. Use this command to add the API Endpoint cmdlets: Add-PSSnapIn Microsoft.Dynamics.Marketing.API
3. Use this command to add the OData Endpoint cmdlets: Add-PSSnapIn Microsoft.Dynamics.Marketing.OData
4. Find all commands with this command: Get-Command -CommandType:Cmdlet -Name:*MDM*

Information will be added to the project Wiki that explain how to use the cmdlets in order to connect to an Microsoft Dynamics Marketing instance and contains usage samples.
