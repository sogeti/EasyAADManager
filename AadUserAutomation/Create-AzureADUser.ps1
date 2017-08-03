workflow Create-AzureADUser
###########################################################
#
# Create Azure AD Users using Azure Automation
#
#
###########################################################
{
    param (
        [object]$WEBHOOKDATA
    )

    Write-output "Fill variables."
    $UsageLocation="NL" 
    $UPNDomainName = "" #Azure AD Domain Name
    $JobIDValue = $PsPrivateMetaData.JobId.Guid
    $ResourceGroup = "" #Resource Group of the Storage Account (logging)
    $StorageAccountName = "" #Storage Account Name
    $LicenseName = ":AAD_PREMIUM" #License SKU

    $StorageAccountKey = '' #Storage Account key

    #Create Eventlog file
    Write-output "Create logfile."
    New-Item -Name $JobIDValue
    Add-Content C:\$JobIDValue "Create Eventlog file."

    # If runbook was called from Webhook, WebhookData will not be null.
    if ($WEBHOOKDATA -ne $null)
    {
        # Collect properties of WebhookData
        $WebhookName    = $WEBHOOKDATA.WebhookName
        $WebhookHeaders = $WEBHOOKDATA.RequestHeader
        $WebhookBody    = $WEBHOOKDATA.RequestBody

        # Collect individual headers. Mail converted from JSON.
        $From = $WebhookHeaders.From
        $CSVUsers = (ConvertFrom-Json -InputObject $WebhookBody)

        Foreach($User in $CSVUsers)
            {
                Write-output "Load FirstName, LastName, UPN, Suffix."

                [string]$FirstName = InlineScript
                                    {
                $CSVUser = $Using:User
                $FirstName = $CSVUser.FirstName
                 
                return $FirstName
            }
                [string]$UPN = InlineScript
                                    {
                $CSVUser = $Using:User
                $UPN = $CSVUser.UPN
                            
                return $UPN
            } 
                [string]$LastName = InlineScript
                                    {
                $CSVUser = $Using:User
                $LastName = $CSVUser.LastName
                            
                return $LastName
            } 
                [string]$Suffix = InlineScript
                                    {
                $CSVUser = $Using:User
                $Suffix = $CSVUser.Suffix
                            
                return $Suffix
            } 
                [string]$Department = InlineScript
                                    {
                $CSVUser = $Using:User
                $Department = $CSVUser.Department
                            
                return $Department
            }

                [string]$DisplayName = InlineScript
                                    {
                $CSVUser = $Using:User
                $DisplayName = $CSVUser.DisplayName
                            
                return $DisplayName
            }          

            InlineScript
                {
                #Function defenitions
                ## Error Handling Function
                Write-output "Load function Error-Handling."
                Function Error-Handling {
                    Param (
                        [Parameter(Mandatory=$true)]
                        [string] 
                        $Description,

                        [Parameter(Mandatory=$true)]
                        [ValidateSet("Continue","Exit")]
                        [string] 
                        $Action
                        )

                    Write-Output "$Description`nError Message:        $($_.Exception.Message)`nError Category Info:  $($Error[0].CategoryInfo)`n"
                    Add-Content C:\$Using:JobIDValue  "$Description
                    Error Message:        $($_.Exception.Message)
                    Error Category Info:  $($Error[0].CategoryInfo)`n"

                    Write-Output "Upload logfile $Using:JobIDValue."
                    Add-Content C:\$Using:JobIDValue "Upload logfile $Using:JobIDValue."

                    $CredentialsStor = Get-AutomationPSCredential -Name 'useradministration'
                    Login-AzureRmAccount -Credential $CredentialsStor

                    $StorageContext = New-AzureStorageContext -StorageAccountName $using:StorageAccountName -StorageAccountKey $Using:StorageAccountKey
                    $StorageContext
                    Set-AzureStorageBlobContent -Context $StorageContext -Container "logfiles" -File C:\$Using:JobIDValue

                    $Action # Check different errors, if create multiple users, continue
                    
                    If ($Action -eq "Exit")
                        {
                        Exit
                        }
                    }

                ## Connect-MsolService Function
                Write-output "Load function Function-ConnectMsolService."
                Function Function-ConnectMsolService {
                    Try {
                        Add-Content C:\$Using:JobIDValue "Connect MS Online service."
                        Write-Output "Connect MS Online service"

                        $Credentials = Get-AutomationPSCredential -Name 'useradministration'
                        Connect-MsolService -Credential $Credentials -ErrorAction Stop
                        }
                    Catch
                        {    
                        Error-Handling -Description "Failed to connect MS Online Service." -Action Exit
                        }
                    }

                ## Login-AzureRmAccount Function
                Write-output "Load function Function-LoginAzureRmAccount."
                Function Function-LoginAzureRmAccount {
                    Try {
                        Add-Content C:\$Using:JobIDValue "Connect Azure RM tenant."
                        Write-Output "Connect Azure RM tenant"

                        $Credentials = Get-AutomationPSCredential -Name 'useradministration'
                        Login-AzureRmAccount -Credential $Credentials -ErrorAction Stop
                        }
                    Catch
                        {    
                        Error-Handling -Description "Failed to connect Azure RM tenant." -Action Exit
                        }
                    }

                Write-Output "Connect-MsolService"
                Function-ConnectMsolService
                Function-LoginAzureRmAccount

                #Start check and create user
                If (!(Get-MsolUser -UserPrincipalName $Using:UPN -erroraction SilentlyContinue))
                    {
                    function New-SWRandomPassword
                        {
                <#
                    .Synopsis
                    Generates one or more complex passwords designed to fulfill the requirements for Active Directory
                    .DESCRIPTION
                    Generates one or more complex passwords designed to fulfill the requirements for Active Directory
                    .EXAMPLE
                    New-SWRandomPassword
                    C&3SX6Kn

                    Will generate one password with a length between 8  and 12 chars.
                    .EXAMPLE
                    New-SWRandomPassword -MinPasswordLength 8 -MaxPasswordLength 12 -Count 4
                    7d&5cnaB
                    !Bh776T"Fw
                    9"C"RxKcY
                    %mtM7#9LQ9h

                    Will generate four passwords, each with a length of between 8 and 12 chars.
                    .EXAMPLE
                    New-SWRandomPassword -InputStrings abc, ABC, 123 -PasswordLength 4
                    3ABa

                    Generates a password with a length of 4 containing atleast one char from each InputString
                    .EXAMPLE
                    New-SWRandomPassword -InputStrings abc, ABC, 123 -PasswordLength 4 -FirstChar abcdefghijkmnpqrstuvwxyzABCEFGHJKLMNPQRSTUVWXYZ
                    3ABa

                    Generates a password with a length of 4 containing atleast one char from each InputString that will start with a letter from
                    the string specified with the parameter FirstChar
                    .OUTPUTS
                    [String]
                    .NOTES
                    Written by Simon Wåhlin, blog.simonw.se
                    I take no responsibility for any issues caused by this script.
                    .FUNCTIONALITY
                    Generates random passwords
                    .LINK
                    http://blog.simonw.se/powershell-generating-random-password-for-active-directory/

                #>
                [CmdletBinding(DefaultParameterSetName = 'FixedLength',ConfirmImpact = 'None')]
                [OutputType([String])]
                Param
                (
                    # Specifies minimum password length
                    [Parameter(Mandatory = $false,
                    ParameterSetName = 'RandomLength')]
                    [ValidateScript({
                                $_ -gt 0
                            }
                    )]
                    [Alias('Min')]
                    [int]$MinPasswordLength = 8,
        
                    # Specifies maximum password length
                    [Parameter(Mandatory = $false,
                    ParameterSetName = 'RandomLength')]
                    [ValidateScript({
                                if($_ -ge $MinPasswordLength)
                                {
                                    $true
                                }
                                else
                                {
                                    Throw 'Max value cannot be lesser than min value.'
                                }
                            }
                    )]
                    [Alias('Max')]
                    [int]$MaxPasswordLength = 12,

                    # Specifies a fixed password length
                    [Parameter(Mandatory = $false,
                    ParameterSetName = 'FixedLength')]
                    [ValidateRange(1,2147483647)]
                    [int]$PasswordLength = 8,
        
                    # Specifies an array of strings containing charactergroups from which the password will be generated.
                    # At least one char from each group (string) will be used.
                    [String[]]$InputStrings = @('abcdefghijkmnpqrstuvwxyz', 'ABCEFGHJKLMNPQRSTUVWXYZ', '23456789', '!$#%&'),

                    # Specifies a string containing a character group from which the first character in the password will be generated.
                    # Useful for systems which requires first char in password to be alphabetic.
                    [String] $FirstChar,
        
                    # Specifies number of passwords to generate.
                    [ValidateRange(1,2147483647)]
                    [int]$Count = 1
                )
                Begin {
                    Function Get-Seed
                    {
                        # Generate a seed for randomization
                        $RandomBytes = New-Object -TypeName 'System.Byte[]' -ArgumentList 4
                        $Random = New-Object -TypeName 'System.Security.Cryptography.RNGCryptoServiceProvider'
                        $Random.GetBytes($RandomBytes)
                        [BitConverter]::ToUInt32($RandomBytes, 0)
                    }
                }
                Process {
                    For($iteration = 1;$iteration -le $Count; $iteration++)
                    {
                        $Password = @{}
                        # Create char arrays containing groups of possible chars
                        [char[][]]$CharGroups = $InputStrings

                        # Create char array containing all chars
                        $AllChars = $CharGroups | ForEach-Object -Process {
                            [Char[]]$_
                        }

                        # Set password length
                        if($PSCmdlet.ParameterSetName -eq 'RandomLength')
                        {
                            if($MinPasswordLength -eq $MaxPasswordLength)
                            {
                                # If password length is set, use set length
                                $PasswordLength = $MinPasswordLength
                            }
                            else
                            {
                                # Otherwise randomize password length
                                $PasswordLength = ((Get-Seed) % ($MaxPasswordLength + 1 - $MinPasswordLength)) + $MinPasswordLength
                            }
                        }

                        # If FirstChar is defined, randomize first char in password from that string.
                        if($PSBoundParameters.ContainsKey('FirstChar'))
                        {
                            $Password.Add(0,$FirstChar[((Get-Seed) % $FirstChar.Length)])
                        }
                        # Randomize one char from each group
                        Foreach($Group in $CharGroups)
                        {
                            if($Password.Count -lt $PasswordLength)
                            {
                                $Index = Get-Seed
                                While ($Password.ContainsKey($Index))
                                {
                                    $Index = Get-Seed
                                }
                                $Password.Add($Index,$Group[((Get-Seed) % $Group.Count)])
                            }
                        }

                        # Fill out with chars from $AllChars
                        for($i = $Password.Count;$i -lt $PasswordLength;$i++)
                        {
                            $Index = Get-Seed
                            While ($Password.ContainsKey($Index))
                            {
                                $Index = Get-Seed
                            }
                            $Password.Add($Index,$AllChars[((Get-Seed) % $AllChars.Count)])
                        }
                        Write-Output -InputObject $(-join ($Password.GetEnumerator() |
                                Sort-Object -Property Name |
                        Select-Object -ExpandProperty Value))
                    }
                }
            }
                    Write-Output -InputObject "Creating User: $Using:DisplayName."
                    Add-Content C:\$Using:JobIDValue "Creating user: $Using:DisplayName."

                    #Create User
                    $GenPassword = New-SWRandomPassword -PasswordLength 12
                    
                    Try
                        {
                        Try
                            {
                            Add-Content C:\$Using:JobIDValue "Create user $Using:DisplayName, $Using:UPN in Azure AD."
                            New-MsolUser -FirstName $using:Firstname `
                                -LastName $using:LastName `
                                -ForceChangePassword $true `
                                -UsageLocation $Using:UsageLocation `
                                -Password  $GenPassword `
                                -UserPrincipalName $Using:UPN `
                                -DisplayName $Using:DisplayName `
                                -ErrorAction Stop

                            Add-Content C:\$Using:JobIDValue "User $Using:DisplayName, $Using:UPN created in Azure AD."
                            }
                        Catch
                            {
                            Error-Handling -Description "Failed to create user $Using:DisplayName, $Using:UPN." -Action Exit
                            }
                        
                        Write-output "Add users to group."
                        # Add users to 2 groups: DEP Everyone and extra group
                        Try
                            {
                            #Retriev Object ID user
                            $userobjectID = Get-MsolUser -UserPrincipalName $Using:UPN -ErrorAction stop
                            
                            Write-Output "Retreived user: $($userobjectID.DisplayName)."
                            Add-Content C:\$Using:JobIDValue "Retreived user: $($userobjectID.DisplayName)."
                            
                            #Retrieve Object ID default groups
                            $DefaultGrp1ID = Get-MsolGroup | Where-Object {$_.Displayname -eq "Dep Everyone"} -ErrorAction stop

                            Write-output "Add user $Using:DisplayName, $Using:UPN to $($DefaultGrp1ID.DisplayName)."
                            Add-Content C:\$Using:JobIDValue "Add user $Using:DisplayName, $Using:UPN to $($DefaultGrp1ID.DisplayName)."

                            #Assign default groups
                            Add-MsolGroupMember -GroupObjectId $DefaultGrp1ID.ObjectID -GroupMemberType "User" -GroupMemberObjectId $userobjectID.ObjectID -ErrorAction stop

                            Write-Output "Succesfully add user $Using:DisplayName, $Using:UPN to $($DefaultGrp1ID.DisplayName)."
                            Add-Content C:\$Using:JobIDValue "Succesfully add user $Using:DisplayName, $Using:UPN to $($DefaultGrp1ID.DisplayName)."

                            $DefaultGrp2ID = $Using:Department

                            If ($DefaultGrp2ID)
                                {
                                #Retrieve Object ID Department group
                                $DefaultGrp2ID = Get-MsolGroup | Where-Object {$_.Displayname -eq "DEP $Using:Department"} -ErrorAction stop

                                Write-output "Add user $Using:DisplayName, $Using:UPN to $($DefaultGrp2ID.DisplayName)."
                                Add-Content C:\$Using:JobIDValue "Add user $Using:DisplayName, $Using:UPN to $($DefaultGrp2ID.DisplayName)."
                                #Assign Department group
                                Add-MsolGroupMember -GroupObjectId $DefaultGrp2ID.ObjectID -GroupMemberType "User" -GroupMemberObjectId $userobjectID.ObjectID -ErrorAction stop
                                
                                Write-Output "Succesfully add user $Using:DisplayName, $Using:UPN to $($DefaultGrp2ID.DisplayName)."
                                Add-Content C:\$Using:JobIDValue "Succesfully add user $Using:DisplayName, $Using:UPN to $($DefaultGrp2ID.DisplayName)."
                                }
                            }
                        Catch
                            {
                            Error-Handling -Description "Failed to add user $Using:DisplayName, $Using:UPN to group." -Action Exit
                            }
                        
                        # Add license to user
                        Try
                            {
                            Add-Content C:\$Using:JobIDValue "Assign license to user $Using:DisplayName, $Using:UPN."
                            #Assign License
                            Set-MsolUserLicense -UserPrincipalName $Using:UPN -AddLicenses $Using:LicenseName

                            Add-Content C:\$Using:JobIDValue "Succesfully assigned license to user $Using:DisplayName, $Using:UPN."
                            }
                        Catch
                            {
                            Error-Handling -Description "Failed to add license to user $Using:DisplayName, $Using:UPN." -Action Exit
                            }

                        }
                    Catch
                        {
                        Error-Handling -Description "Failed to complete create user $Using:DisplayName, $Using:UPN." -Action Exit
                        }                  
                    }
                    Else
                    {
                    Add-Content C:\$Using:JobIDValue "User $Using:DisplayName, $Using:UPN already exists."
                    }
                
                Write-Output "Upload logfile $Using:JobIDValue."
                Add-Content C:\$Using:JobIDValue "Upload logfile $Using:JobIDValue."

                $CredentialsStor = Get-AutomationPSCredential -Name 'useradministration'
                Login-AzureRmAccount -Credential $CredentialsStor

                #$Key = (Get-AzureRmStorageAccountKey -ResourceGroupName $Using:ResourceGroup -StorageAccountName $Using:StorageAccountName).item(0).value

                $StorageContext = New-AzureStorageContext -StorageAccountName $using:StorageAccountName -StorageAccountKey $using:StorageAccountKey
                $StorageContext
                Set-AzureStorageBlobContent -Context $StorageContext -Container "logfiles" -File C:\$Using:JobIDValue#>
                }
        }
    }
}