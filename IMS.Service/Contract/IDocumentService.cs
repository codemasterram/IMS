using IMS.Data;
using IMS.Logic.ViewModels.BackOffice.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Logic.Contract
{
    public interface IDocumentService
    {
        DocumentTypeModel GetDocumentType(int documentTypeId);
        IList<DocumentTypeModel> GetDocumentType(NTAEnum.eModule module, NTAEnum.eDocumentModule documentModule = NTAEnum.eDocumentModule.None);
        IList<DocumentFileMetaDataModel> GetFileMetaData(int fileId, int masterId, int dataId, NTAEnum.eDocumentType docType, NTAEnum.eModule module);
        IList<DocumentDataModel> GetDataListForDocumentType(int masterId, NTAEnum.eDocumentType docType);
        Task<DocumentFileModel> GetFilePreviewAsync(int fileId, NTAEnum.eModule module, NTAEnum.ePreviewSize size);
        Task<DocumentFileModel> GetFileAsync(int fileId, NTAEnum.eModule module);
        Task<int> SaveFileAsync(int masterId, int dataId, NTAEnum.eDocumentType docType, HttpPostedFileBase fileData, string remarks, int currentUserId);
        Task<int> RemoveFileAsync(int fileId, int currentUserId);
        void AddFileRemarks(int fileId, string remarks);
    }
}
