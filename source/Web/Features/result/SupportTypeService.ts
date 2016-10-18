module ICanHasDotnetCore.Result.SupportTypeService {

    export interface IColours {
        border: string;
        background: string;
        highlight: {
            border: string;
            background: string;
        }
    }

    export interface IService {
        getColours(type: SupportType): IColours;
        getDisplayName(type: SupportType): string;
    }

    class Service implements IService {
        getColours(type: SupportType): IColours {
            switch (type) {
                case SupportType.NotFound: //Grey
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
                case SupportType.NoDotNetLibraries: //blue grey
                    return { border: "#78909C", background: "#B0BEC5", highlight: { border: "#78909C", background: "#CFD8DC" } };
                case SupportType.Error: // red
                    return { border: "#b71c1c", background: "#ef9a9a", highlight: { border: "#b71c1c", background: "#ffcdd2" } };
                default: // B&W
                    return { border: "#212121", background: "#FAFAFA", highlight: { border: "#212121", background: "#FAFAFA" } };
            }
        };

        getDisplayName(type: SupportType): string {
            switch (type) {
                case SupportType.NotFound:
                    return "Not Found";
                case SupportType.KnownReplacementAvailable: 
                    return "Known Replacement Available";
                case SupportType.InvestigationTarget: 
                    return "Your Project";
                case SupportType.Supported: 
                    return "Supported";
                case SupportType.PreRelease:
                    return "Supported (Pre-release)";
                case SupportType.NoDotNetLibraries:
                    return "Not a .NET Library";
                case SupportType.Unsupported:
                    return "Unsupported";
                case SupportType.Error:
                    return "Error";
                default: 
            }
        };
    }

    app.service("supportTypeService", Service);
}