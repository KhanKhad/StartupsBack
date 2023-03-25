using StartupsBack.ViewModels.ActionsResults;

namespace StartupsBack.Models.JsonModels
{
    public class StartupJsonModel
    {
        #region RequestProperties
        public string AuthorToken { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        #endregion

        public StartupJsonModel() 
        {

        }

        public StartupJsonModel(StartupCreateResult startupCreateResult)
        {
            StartupCreateResult = startupCreateResult.StartupCreateResultType;
            ErrorOrEmpty = startupCreateResult.ErrorOrNull?.Message ?? string.Empty;
            StartupId = startupCreateResult.StartupOrNull?.Id ?? -1;
        }

        #region ResponseProperties
        public int StartupId { get; set; } = -1;
        public StartupCreateResultType StartupCreateResult { get; set; }
        public string ErrorOrEmpty { get; set; } = string.Empty;
        #endregion
    }
}
