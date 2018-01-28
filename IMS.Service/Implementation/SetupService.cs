using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Contract;
using System.Collections.Generic;
using System.Linq;
using IMS.Logic.Configuration;
using IMS.Logic.Utilities;
using IMS.Logic.ViewModels.BackOffice.Common;

namespace IMS.Logic.Implementation
{
    public class SetupService : ISetupService
    {
        #region Constructor
        private IRepository<DocumentNumbering> documentNumberingRepository;
        private IExceptionService exceptionService;
        public SetupService(IRepository<DocumentNumbering> documentNumberingRepo,
             IExceptionService exceptionSVC)
        {
            documentNumberingRepository = documentNumberingRepo;
            exceptionService = exceptionSVC;
        }
        #endregion

        #region DocumentNumbering

        public DocumentNumberingViewModel GetDocumentNumbering(int documentNumberingId)
        {
            var item = documentNumberingRepository.GetById(documentNumberingId);
            var model = AutomapperConfig.Mapper.Map<DocumentNumberingViewModel>(item);
            return model;
        }

        public IList<DocumentNumberingListViewModel> GetDocumentNumberings(int fiscalYearId, int? documentSetupId)
        {
            var data = documentNumberingRepository.Table.Where(x => (x.FiscalYearId == fiscalYearId)
            && (!documentSetupId.HasValue || x.DocumentSetupId == documentSetupId)).OrderBy(x => x.FiscalYearId);
            var model = AutomapperConfig.Mapper.Map<IList<DocumentNumberingListViewModel>>(data);
            return model;
        }

        #endregion
    }
}
