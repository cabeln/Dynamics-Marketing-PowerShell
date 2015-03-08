# Dynamics Marketing PowerShell
PowerShell command lets for Microsoft Dynamics Marketing

This project showcases how to use the API and the OData endpoint of a Microsoft Dynamics Marketing instance through Microsoft PowerShell. Two modules a compiled from the solution which implement a set of cmdlets that cover the full SDK and allow to use "Find" methods in order to search sets of entities including terms like  filter, sort, skip, top, expand.

1. Install the cmdlet modules on your local machine
2. Use this command to add the API Endpoint cmdlets: Add-PSSnapIn Microsoft.Dynamics.Marketing.API
3. Use this command to add the OData Endpoint cmdlets: Add-PSSnapIn Microsoft.Dynamics.Marketing.OData
4. Find all included commands with this command: Get-Command -CommandType:Cmdlet -Name:*MDM*

Information will be added to the project Wiki that explain how to use the cmdlets in order to connect to an Microsoft Dynamics Marketing instance and contains usage samples.

SDK Commands:
Name                                                               
----                                                               
Add-MDMCompany                                                     
Add-MDMContact                                                     
Add-MDMContacts                                                    
Add-MDMContactToList                                               
Add-MDMCustomFieldCategories                                       
Add-MDMEventAttendance                                             
Add-MDMEventRegistration                                           
Add-MDMExternalEntity                                              
Add-MDMLead                                                        
Add-MDMList                                                        
Add-MDMMarketingResult                                             
Connect-MDMApi                                                     
Copy-MDMListContacts                                               
Disconnect-MDMApi                                                  
Get-MDMAllLists                                                    
Get-MDMApiResponse                                                 
Get-MDMCompanies                                                   
Get-MDMCompany                                                     
Get-MDMContact                                                     
Get-MDMContactPermissions                                          
Get-MDMContacts                                                    
Get-MDMCurrencies                                                  
Get-MDMCustomFields                                                
Get-MDMEmailMessages                                               
Get-MDMEmailMessageSentStatus                                      
Get-MDMEventAttendance                                             
Get-MDMEventAttendanceStatuses                                     
Get-MDMEventRegistration                                           
Get-MDMExternalEntiity                                             
Get-MDMExternalEntity                                              
Get-MDMExternalIds                                                 
Get-MDMHardbounces                                                 
Get-MDMLanguages                                                   
Get-MDMLead                                                        
Get-MDMLeadPriorities                                              
Get-MDMLeads                                                       
Get-MDMLeadStatuses                                                
Get-MDMList                                                        
Get-MDMMarketingResult                                             
Get-MDMMarketingResults                                            
Get-MDMMarketingResultTypes                                        
Get-MDMMissingContactPermissions                                   
Get-MDMSalesRatings                                                
Get-MDMSalutations                                                 
Get-MDMSchemaForEmailMessage                                       
Get-MDMTerritories                                                 
Remove-MDMAllContactsFromList                                      
Remove-MDMCompany                                                  
Remove-MDMContact                                                  
Remove-MDMContactFromList                                          
Remove-MDMEventAttendance                                          
Remove-MDMEventRegistration                                        
Remove-MDMExternalEntity                                           
Remove-MDMExternalEntuityType                                      
Remove-MDMLead                                                     
Remove-MDMList                                                     
Remove-MDMMarketingResult                                          
Send-MDMApiRequest                                                 
Send-MDMEmailMessages                                              
Set-MDMContactPermissions                                          
Set-MDMContactsPermissions                                         
Set-MDMHardbouncesProcessed                        

OData commands:
Name                           
----                           
Connect-MDMOData               
Disconnect-MDMOData            
Find-MDMAnyData                
Find-MDMCompanies              
Find-MDMContacts               
Find-MDMLeads                  
Get-MDMMetadata     
