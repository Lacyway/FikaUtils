$EXEDIR = Get-Location

Get-NetFirewallPortFilter | Where-Object -Property LocalPort -EQ 6969 | Remove-NetFirewallRule
Get-NetFirewallPortFilter | Where-Object -Property LocalPort -EQ 25565 | Remove-NetFirewallRule
Get-NetFirewallApplicationFilter -Program "$EXEDIR\SPT.Server.exe" | Remove-NetFirewallRule
Get-NetFirewallApplicationFilter -Program "$EXEDIR\EscapeFromTarkov.exe" | Remove-NetFirewallRule
Get-NetFirewallApplicationFilter -Program "$EXEDIR\SPT.Launcher.exe" | Remove-NetFirewallRule
Get-NetFirewallApplicationFilter -Program "$EXEDIR\Aki.Server.exe" | Remove-NetFirewallRule
Get-NetFirewallApplicationFilter -Program "$EXEDIR\Aki.Launcher.exe" | Remove-NetFirewallRule
Get-NetFirewallRule -DisplayName "#FIKA*" | Remove-NetFirewallRule