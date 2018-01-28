using IMS.Logic.ViewModels.BackOffice.Common;
using System.Collections.Generic;

namespace IMS.Logic.Contract
{
    public interface ISetupService
    {
        DocumentNumberingViewModel GetDocumentNumbering(int documentNumberingId);
        IList<DocumentNumberingListViewModel> GetDocumentNumberings(int fiscalYearId, int? documentSetupId);
    }
}
