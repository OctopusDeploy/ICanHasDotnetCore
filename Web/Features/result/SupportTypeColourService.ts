module ICanHasDotnetCore.Result.SupportTypeColourService {

    export interface IService {
        get(wasSuccessful: boolean, type: SupportType);
    }

    class Service implements IService {
        get(wasSuccessful: boolean, type: SupportType) {
            if (!wasSuccessful)// red
                return { border: "#b71c1c", background: "#ef9a9a", highlight: { border: "#b71c1c", background: "#ffcdd2" } };

            switch (type) {
                case SupportType.Unknown: //Grey
                    return { border: "#616161", background: "#E0E0E0", highlight: { border: "#616161", background: "#F5F5F5" } };
                case SupportType.KnownReplacementAvailable: // blue
                    return { border: "#0277BD", background: "#81D4FA", highlight: { border: "#0277BD", background: "#B3E5FC" } };
                case SupportType.InvestigationTarget: // purple
                    return { border: "#673AB7", background: "#B39DDB", highlight: { border: "#673AB7", background: "#D1C4E9" } };
                case SupportType.Supported: // green
                    return { border: "#43A047", background: "#A5D6A7", highlight: { border: "#43A047", background: "#C8E6C9" } };
                case SupportType.PreRelease: // teal
                    return { border: "#00897B", background: "#80CBC4", highlight: { border: "#00897B", background: "#B2DFDB" } };
                case SupportType.Unsupported: //orange
                    return { border: "#FF9800", background: "#FFCC80", highlight: { border: "#FF9800", background: "#FFE0B2" } };
                default: // B&W
                    return { border: "#212121", background: "#FAFAFA", highlight: { border: "#212121", background: "#FAFAFA" } };
            }
        };
    }

    app.service("supportTypeColourService", Service);
}