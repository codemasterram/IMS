using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Data;
using IMS.Data.Infrastructure;
using System.Web;
using System.IO;
using IMS.Logic.ViewModels.BackOffice.Common;
using IMS.Logic.Configuration;
using IMS.Logic.Utilities;
using System.Drawing;
using System.Drawing.Imaging;

namespace IMS.Logic.Implementation
{
    public class DocumentService : IDocumentService
    {
        private IRepository<Nominee> _nomineeRepository;
        private IRepository<DocumentType> _docTypeRepository;
        private IRepository<HRDocument> _hrDocumentRepository;
        private IRepository<EducationalQualification> _educationalQualificationRepository;
        private IRepository<CustomerDocument> _customerDocumentRepository;
        public DocumentService(IRepository<DocumentType> docTypeRepository,
            IRepository<HRDocument> hrDocumentRepository,
            IRepository<Nominee> nomineeRepository,
            IRepository<EducationalQualification> educationalQualificationRepository,
            IRepository<CustomerDocument> customerDocumentRepository)
        {
            _nomineeRepository = nomineeRepository;
            _docTypeRepository = docTypeRepository;
            _hrDocumentRepository = hrDocumentRepository;
            _educationalQualificationRepository = educationalQualificationRepository;
            _customerDocumentRepository = customerDocumentRepository;
        }

        public DocumentTypeModel GetDocumentType(int documentTypeId)
        {
            var doc = _docTypeRepository.Table.FirstOrDefault(x => x.Id == documentTypeId);
            return AutomapperConfig.Mapper.Map<DocumentTypeModel>(doc);
        }

        public IList<DocumentTypeModel> GetDocumentType(NTAEnum.eModule module, NTAEnum.eDocumentModule documentModule = NTAEnum.eDocumentModule.None)
        {
            var query = _docTypeRepository.Table.Where(x => x.ModuleId == (int)module);

            if (documentModule != NTAEnum.eDocumentModule.None)
            {
                string docModule = Enum.GetName(typeof(NTAEnum.eDocumentModule), documentModule);
                query = query.Where(x => x.DataModule == docModule).OrderBy(x => x.DisplayOrder);
            }

            var list = AutomapperConfig.Mapper.Map<IList<DocumentTypeModel>>(query);
            list.Insert(0, new DocumentTypeModel { Id = 0, DisplayName = "None" });
            return list;
        }

        public IList<DocumentFileMetaDataModel> GetFileMetaData(int fileId, int masterId, int dataId, NTAEnum.eDocumentType docType, NTAEnum.eModule module)
        {
            var documentType = _docTypeRepository.GetById((int)docType);
            if (Common.GetClientModule.Any(x => x == module))
            {
                var doc = _customerDocumentRepository.Table;

                if (fileId > 0)
                    doc = doc.Where(x => x.Id == fileId);

                if (dataId > 0)
                    doc = doc.Where(x => x.DataId == dataId && x.DocumentTypeId == (int)docType);

                var model = new List<DocumentFileMetaDataModel>();
                if (doc.Any())
                {
                    model = doc.Select(x => new DocumentFileMetaDataModel
                    {
                        FileId = x.Id,
                        MasterId = x.DataId,
                        DataId = x.DataId,
                        FileName = x.FileName,
                        MimeType = x.FileMimeType,
                        Name = documentType.DisplayName,
                        Remarks = "",
                        Module = (int)module
                    }).ToList();
                }

                return model;
            }
            else
            {
                var doc = _hrDocumentRepository.Table;

                if (fileId > 0)
                    doc = doc.Where(x => x.Id == fileId);

                if (masterId > 0)
                    doc = doc.Where(x => x.MasterId == masterId && x.DocumentTypeId == (int)docType);

                if (dataId > 0)
                    doc = doc.Where(x => x.DataId == dataId && x.DocumentTypeId == (int)docType);

                var model = new List<DocumentFileMetaDataModel>();
                if (doc.Any())
                {
                    model = doc.Select(x => new DocumentFileMetaDataModel
                    {
                        FileId = x.Id,
                        MasterId = x.MasterId,
                        DataId = x.DataId,
                        FileName = x.FileName,
                        MimeType = x.FileMimeType,
                        Name = documentType.DisplayName,
                        Remarks = x.Remarks,
                        Module = (int)module
                    }).ToList();
                }

                return model;
            }
        }

        public async Task<int> SaveFileAsync(int masterId, int dataId, NTAEnum.eDocumentType docType, HttpPostedFileBase fileData, string remarks, int currentUserId)
        {
            if (fileData != null)
            {
                MemoryStream stream = new MemoryStream();
                if (fileData != null)
                    fileData.InputStream.CopyTo(stream);

                var query = _hrDocumentRepository.TableUntracked.Where(x => x.DataId == dataId && x.DocumentTypeId == (int)docType);

                var documentType = _docTypeRepository.Table.FirstOrDefault(x => x.Id == (int)docType);
                //validate file extensions
                var validExtensions = documentType.Accept.Split(',');
                var fileInfo = new System.IO.FileInfo(fileData.FileName);
                if (!validExtensions.Any(x => fileInfo.Extension.ToLower() == x.ToLower()))
                    return await Task.Run<int>(() => { return 0; });

                if (query.Any() && !query.FirstOrDefault().DocumentType.IsMultipleAllowed)
                {
                    _hrDocumentRepository.Delete(query.FirstOrDefault());
                }

                var document = new HRDocument
                {
                    DocumentTypeId = (int)docType,
                    MasterId = masterId,
                    DataId = dataId,
                    FileBinary = stream.ToArray(),
                    FileName = System.IO.Path.GetFileName(fileData.FileName),
                    FileMimeType = fileData.ContentType,
                    FileSize = fileData.ContentLength,
                    Remarks = remarks,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = currentUserId,
                    LastModifiedOn = DateTime.Now
                };

                if (fileData.ContentType.Contains("image"))
                {
                    Bitmap bmp = new Bitmap(stream);

                    //create thumb
                    var thumb1 = Common.CreateThumb(bmp, 60, 60);
                    MemoryStream stream1 = new MemoryStream();
                    thumb1.Save(stream1, ImageFormat.Jpeg);
                    document.ThumbSmall = stream1.ToArray();

                    var thumb2 = Common.CreateThumb(bmp, 100, 100);
                    MemoryStream stream2 = new MemoryStream();
                    thumb2.Save(stream2, ImageFormat.Jpeg);
                    document.ThumbMiddle = stream2.ToArray();
                }

                _hrDocumentRepository.Insert(document);
                stream.Dispose();
                return await Task.Run<int>(() => { return document.Id; });
            }

            return await Task.Run<int>(() => { return 0; });
        }

        public async Task<int> RemoveFileAsync(int fileId, int currentUserId)
        {
            var document = _hrDocumentRepository.Table.FirstOrDefault(x => x.Id == fileId);
            if (document != null)
                _hrDocumentRepository.Delete(document);

            return await Task.Run<int>(() => { return 0; });
        }

        public async Task<DocumentFileModel> GetFilePreviewAsync(int fileId = 0, NTAEnum.eModule module = NTAEnum.eModule.None, NTAEnum.ePreviewSize size = NTAEnum.ePreviewSize.Middle)
        {
            return await Task.Factory.StartNew<DocumentFileModel>(() =>
            {
                var model = new DocumentFileModel();

                if (Common.GetClientModule.Any(x => x == module))
                {
                    var document = _customerDocumentRepository.Table.Where(x => x.Id == fileId);
                    if (document.Any())
                    {
                        model = document.Select(x => new DocumentFileModel
                        {
                            DataId = x.DataId,
                            Name = x.DocumentType.DisplayName,
                            MimeType = x.FileMimeType,
                            FileData = x.FileMimeType.Contains("image") ? x.FileBinary : null
                        }).FirstOrDefault();
                    }

                    return model;
                }
                else
                {
                    var document = _hrDocumentRepository.Table.Where(x => x.Id == fileId);
                    if (document.Any())
                    {
                        model = document.Select(x => new DocumentFileModel
                        {
                            DataId = x.DataId,
                            Name = x.DocumentType.DisplayName,
                            MimeType = x.FileMimeType,
                            FileData = x.FileMimeType.Contains("image") ? x.FileBinary : null
                        }).FirstOrDefault();
                    }

                    return model;
                }
            });
        }

        public async Task<DocumentFileModel> GetFileAsync(int fileId, NTAEnum.eModule module)
        {
            return await Task.Factory.StartNew<DocumentFileModel>(() =>
            {
                var model = new DocumentFileModel();

                if (Common.GetClientModule.Any(x => x == module))
                {
                    var document = _customerDocumentRepository.Table.Where(x => x.Id == fileId);
                    if (document.Any())
                    {
                        model = document.Select(x => new DocumentFileModel
                        {
                            DataId = x.DataId,
                            Name = x.DocumentType.DisplayName,
                            MimeType = x.FileMimeType,
                            FileData = x.FileBinary
                        }).FirstOrDefault();
                    }

                    return model;
                }
                else
                {
                    var document = _hrDocumentRepository.Table.Where(x => x.Id == fileId);
                    if (document.Any())
                    {
                        model = document.Select(x => new DocumentFileModel
                        {
                            DataId = x.DataId,
                            Name = x.DocumentType.DisplayName,
                            MimeType = x.FileMimeType,
                            FileData = x.FileBinary
                        }).FirstOrDefault();
                    }

                    return model;
                }
            });
        }

        public void AddFileRemarks(int fileId, string remarks)
        {
            var doc = _hrDocumentRepository.GetById(fileId);
            doc.Remarks = remarks;
            _hrDocumentRepository.Update(doc);
        }

        public IList<DocumentDataModel> GetDataListForDocumentType(int masterId, NTAEnum.eDocumentType docType)
        {
            IList<DocumentDataModel> dataList = new List<DocumentDataModel>();
            switch (docType)
            {
                case NTAEnum.eDocumentType.EmployeeEducationalCertificate:
                    dataList = _educationalQualificationRepository.Table.Where(x => x.EmployeeId == masterId).Select(x => new DocumentDataModel { Id = x.Id, Name = x.Degree.Name }).ToList();
                    break;
            }

            return dataList;
        }
    }
}
