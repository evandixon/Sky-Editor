Namespace Extensions
    Public Enum ExtensionInstallResult
        Success
        RestartRequired
        InvalidFormat 'For when the zip doesn't contain the correct info.
        UnsupportedFormat 'For when the extension type is unsupported
        IncompatiblePlatform 'For when the extension isn't supported on the current platform of Sky Editor (e.g. a WPF UI plugin on Android).
    End Enum
End Namespace

