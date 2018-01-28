using IMS.Data;
using IMS.Logic.ViewModels;
using IMS.Logic.ViewModels.BackOffice.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Web.Mvc;
using static IMS.Data.NTAEnum;
using IMS.Logic.ViewModels.BackOffice.Common;
using IMS.Data.Infrastructure;
using IMS.Logic.ViewModels.BackOffice.IMS;

namespace IMS.Logic.Contract
{
    public interface ICommonService
    {
        IList<SystemConfigurationViewModel> GetSystemConfiguration(string key = null);

        FiscalYearViewModel GetCurrentFiscalYear();

        IList<FiscalYearViewModel> GetFiscalYearList();

        SelectList GetDropDownList<T>(string dataValueField, string dataTextField, object selectedValue = null) where T : class;

        SelectList GetDropDownList<T>(IEnumerable<T> list, string dataValueField, string dataTextField, object selectedValue = null) where T : class;

        SelectList GetDropDownList(eSelectListType listType, object selectedValue = null, object pushItemBefore = null);

        SelectList GetDistrictList(int? zoneId, string dataValueField, string dataTextField, object selectedValue = null);

        SelectList GetVdcList(int? districtId, string dataValueField, string dataTextField, object selectedValue = null);

        IMS.Data.Infrastructure.Department GetSection(int id);

        SelectList GetItemList(int? itemGroupId, object selectedValue = null);

        ItemStatusViewModel GetItemStatusByItemId(int itemId, DateTime date, int fiscalYearId);

        SelectList GetEmployeeList(int? sectionId, object selectedValue = null);

        SelectList GetConsumableItems(object selectedValue = null);

        SelectList GetNonConsumableItems(object selectedValue = null);

       
        
        #region TestLog
        TestLogViewModel SaveTestLog(TestLogViewModel model);
        TestLogViewModel MakeAsResolve(TestLogViewModel model);
        TestLogViewModel GetTestLog(int TestLogId);
        IPagedList<TestLogListViewModel> GetTestLogs(string module = null, string dateFromBs = null, string dateToBS = null, bool pending = true, int? page = null, int? pageSize = null);
        ServiceModel DeleteTestLog(int id);
        #endregion TestLog
    }
}
