export interface Registry {
    MachineName: string;
    IP: string;
    Antivirus: string;
    WindowsVersion: string;
    DotNetVersion: string;
    Disk: any;
    FirewallStatus: string[];

    Collapsed: boolean;
    Checked: boolean;
    On: boolean;
}
