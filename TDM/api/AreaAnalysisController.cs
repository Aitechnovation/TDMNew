﻿using AutoMapper;
using Microsoft.SqlServer.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TDM.Models;

namespace TDM.Controllers.api
{
    public class AreaAnalysisController : ApiController
    {
        private JsonSerializerSettings jsonSetting = JsonHelper.createJsonSetting();
        private TDManagementEntities tdmEntities = null;
        private TDASSETEntities tdaEntities = null;

        protected AreaAnalysisController()
        {
            tdmEntities = new TDManagementEntities();
            tdaEntities = new TDASSETEntities();
            tdmEntities.Configuration.ProxyCreationEnabled = false;
            tdaEntities.Configuration.ProxyCreationEnabled = false;
        }

        //[HttpPost]
        //public IHttpActionResult AddProvImpact(AddProvImpactPARAMS prms)
        //{
        //    try
        //    {
        //        int project_id = prms.projectId;
        //        string[] prov_codes = prms.provCodes;

        //        var add_list = prov_codes.Select(x => new PROV_IMPACT() {
        //            ID = 0,
        //            PROJECT_ID = project_id,
        //            PROV_CODE = x
        //        }).ToList();

        //        var added_list = tdmEntities.PROV_IMPACT.AddRange(add_list);
        //        tdmEntities.SaveChanges();
        //        return Json(added_list, jsonSetting);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }
        //}
        //public class AddProvImpactPARAMS {
        //    public int projectId { get; set; }
        //    public string[] provCodes { get; set; }
        //}

        //[HttpPost]
        //public IHttpActionResult DelProvImpact(DelProvImpactPARAMS prms)
        //{
        //    try
        //    {
        //        int[] ids = prms.ids;
        //        var founds = tdmEntities.PROV_IMPACT.Where(x => ids.Any(y=>y==x.ID));
        //        var removes = tdmEntities.PROV_IMPACT.RemoveRange(founds);

        //        tdmEntities.SaveChanges();
        //        return Json(removes, jsonSetting);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }
        //}
        //public class DelProvImpactPARAMS
        //{
        //    public int[] ids { get; set; }
        //}

        [HttpPost]
        public IHttpActionResult UpdateProject(PROJECT_IMPACT project)
        {
            try
            {
                PROJECT_IMPACT updateProject = tdmEntities.PROJECT_IMPACT.First(x => x.ID == project.ID);

                updateProject.IS_DELETED = project.IS_DELETED;
                updateProject.SUBJECT_ID = project.SUBJECT_ID;
                updateProject.SUBJECT_NAME = project.SUBJECT_NAME;
                updateProject.PUBLISH_DATE = project.PUBLISH_DATE;
                updateProject.IS_PUBLISHED = project.IS_PUBLISHED;
                updateProject.UPDATE_BY = project.UPDATE_BY;
                updateProject.UPDATE_DATE = project.UPDATE_DATE;
                updateProject.CREATE_DATE = project.CREATE_DATE;
                updateProject.CREATE_BY = project.CREATE_BY;

                tdmEntities.SaveChanges();

                return Json(project, jsonSetting);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult AddProject(PROJECT_IMPACT project)
        {
            try
            {
                project.ID = 0;
                PROJECT_IMPACT saveProject = tdmEntities.PROJECT_IMPACT.Add(project);
                tdmEntities.SaveChanges();
                return Json(saveProject, jsonSetting);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteProject(DelProjectImpact project)
        {
            try
            {
                PROJECT_IMPACT deleteProject = tdmEntities.PROJECT_IMPACT.First(x => x.ID == project.ID);
                deleteProject.ID = project.ID;
                deleteProject.IS_DELETED = project.IS_DELETED;

                tdmEntities.SaveChanges();
                return Json(project, jsonSetting);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public class DelProjectImpact
        {
            public int ID { get; set; }
            public bool IS_DELETED { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetProjectArea(string subject_id)
        {
            try
            {
                List<PROJECT_AREA_ViewModel> projectArea = tdaEntities.PROJECT_AREA_47.Where(x => x.SUBJECT_ID.Equals(subject_id)).Select(x => Mapper.Map<PROJECT_AREA_ViewModel>(x)).ToList();
                projectArea.AddRange(tdaEntities.PROJECT_AREA_48.Where(x => x.SUBJECT_ID.Equals(subject_id)).Select(x => Mapper.Map<PROJECT_AREA_ViewModel>(x)).ToList());

                return Json(projectArea.Select(x => new
                {
                    x.OBJECTID,
                    x.SUBJECT_ID,
                    x.PROV_CODE,
                    x.STATUS_PROCESS,
                    SHAPE = x.SHAPE.ToString()
                }).ToList());
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllProvince()
        {
            try
            {
                return Json(tdaEntities.PROVINCEs.Select(x => new
                {
                    CODE = x.PRO_C,
                    NAME = x.ON_PRO_THA
                }).ToList());
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllProjectImpact(int start, int count, string subject_id = "", string subject_name = "", string prov_name = "", DateTime? publish_date = null)
        {
            try
            {
                var PROVINCES = tdaEntities.PROVINCEs.Where(p => p.NAME_T.Contains(prov_name));

                var _projects = tdmEntities.PROJECT_IMPACT.Where(x => !x.IS_DELETED).ToList();
                var projects = _projects.Where(x =>
                    (subject_id == null || x.SUBJECT_ID.Contains(subject_id))
                    && (subject_name == null || x.SUBJECT_NAME.Contains(subject_name))
                    && (prov_name == null || prov_name.Length == 0 || (PROVINCES.Count() > 0 && (
                        tdaEntities.PROJECT_AREA_47.Any(y => y.SUBJECT_ID.Equals(x.SUBJECT_ID) && PROVINCES.Any(z => z.PRO_C.Equals(y.PROV_CODE)))
                        || tdaEntities.PROJECT_AREA_48.Any(y => y.SUBJECT_ID.Equals(x.SUBJECT_ID) && PROVINCES.Any(z => z.PRO_C.Equals(y.PROV_CODE))))))
                    && (publish_date == null || (x.PUBLISH_DATE.HasValue && x.PUBLISH_DATE.Value.Date == publish_date.Value.Date))
                    ).ToList().Select(x => Mapper.Map<PROJECT_IMPACT_ViewModel>(x)).ToList();

                List<dynamic> result = new List<dynamic>();
                foreach (var c in projects)
                {
                    List<PROJECT_AREA_ViewModel> projectArea = tdaEntities.PROJECT_AREA_47.Where(x => x.SUBJECT_ID.Equals(c.SUBJECT_ID)).ToList().Select(x => Mapper.Map<PROJECT_AREA_ViewModel>(x)).ToList();
                    projectArea.AddRange(tdaEntities.PROJECT_AREA_48.Where(x => x.SUBJECT_ID.Equals(c.SUBJECT_ID)).ToList().Select(x => Mapper.Map<PROJECT_AREA_ViewModel>(x)).ToList());
                    c.PROVINCE = projectArea.GroupBy(x => x.PROV_CODE).Select(x => Mapper.Map<PROVINCE_ViewModel>(tdaEntities.PROVINCEs.Where(y => y.PRO_C.Equals(x.Key)).First())).ToList();
                    int statusId = projectArea.Count() == 0 ? 1 : (projectArea.Any(x => x.STATUS_PROCESS.Equals("N")) ? 2 : 3);
                    dynamic project = new
                    {
                        c.ID,
                        c.SUBJECT_ID,
                        c.SUBJECT_NAME,
                        c.PUBLISH_DATE,
                        c.CREATE_DATE,
                        c.CREATE_BY,
                        c.UPDATE_DATE,
                        c.UPDATE_BY,
                        c.PROVINCE,
                        c.IS_PUBLISHED,
                        STATUS = tdmEntities.STATUS_IMPACT.Where(y => y.ID == statusId).First()
                    };
                    result.Add(project);
                }

                result = result.Skip(start).Take(count).ToList();
                return Json(result, jsonSetting);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        public class GetAllProjectImpactPrms
        {
            public int start { get; set; }
            public int count { get; set; }
            public string subject_id { get; set; }
            public string subject_name { get; set; }
            public string prov_name { get; set; }
            public DateTime publish_date { get; set; }
        }

        [HttpGet]
        public IHttpActionResult SumAllProjectImpact(int start, int count, string prov_code = "", string amphur_code = "", string branch_code = "", string subject_name = "")
        {
            try
            {
                var _projects = tdmEntities.PROJECT_IMPACT.Where(x => !x.IS_DELETED && x.IS_PUBLISHED).ToList();
                var projects = _projects.Where(x =>
                    (subject_name == null || x.SUBJECT_NAME.Contains(subject_name))
                    && (prov_code == null || prov_code.Length == 0 || ((
                        tdaEntities.PROJECT_AREA_47.Any(y => y.SUBJECT_ID.Equals(x.SUBJECT_ID) && y.PROV_CODE.Equals(prov_code))
                        || tdaEntities.PROJECT_AREA_48.Any(y => y.SUBJECT_ID.Equals(x.SUBJECT_ID) && y.PROV_CODE.Equals(prov_code)))))
                    ).ToList().Select(x => Mapper.Map<PROJECT_IMPACT_ViewModel>(x)).ToList();

                List<dynamic> result = new List<dynamic>();
                foreach (var c in projects)
                {

                    List<PROJECT_AREA_ViewModel> projectArea = tdaEntities.PROJECT_AREA_47.Where(x => x.SUBJECT_ID.Equals(c.SUBJECT_ID)).ToList().Select(x => Mapper.Map<PROJECT_AREA_ViewModel>(x)).ToList();
                    projectArea.AddRange(tdaEntities.PROJECT_AREA_48.Where(x => x.SUBJECT_ID.Equals(c.SUBJECT_ID)).ToList().Select(x => Mapper.Map<PROJECT_AREA_ViewModel>(x)).ToList());
                    c.PROVINCE = projectArea.GroupBy(x => x.PROV_CODE).Select(x => Mapper.Map<PROVINCE_ViewModel>(tdaEntities.PROVINCEs.Where(y => y.PRO_C.Equals(x.Key)).First())).ToList();
                    int statusId = projectArea.Count() == 0 ? 1 : (projectArea.Any(x => x.STATUS_PROCESS.Equals("N")) ? 2 : 3);
                    var PROJECT_AREA = projectArea.Select(x => new
                    {
                        x.OBJECTID,
                        x.SUBJECT_ID,
                        x.PROV_CODE,
                        x.STATUS_PROCESS,
                        SHAPE = x.SHAPE.ToString()
                    }).ToList();

                    DbGeometry PROJECT_AREA_SHAPE = null;
                    if (projectArea.Count() > 0) PROJECT_AREA_SHAPE = projectArea.ElementAt(0).SHAPE;
                    for (var i = 1; i < projectArea.Count(); i++)
                    {
                        var temp = PROJECT_AREA_SHAPE.Union(projectArea.ElementAt(i).SHAPE);
                        if (temp != null)
                        {
                            PROJECT_AREA_SHAPE = temp;
                        }
                    }

                    if (statusId == 3)
                    {
                        //////////////////////////////////
                        if (amphur_code != null && amphur_code.Length == 0)
                        {
                            amphur_code = null;
                        }

                        if (branch_code != null && branch_code.Length == 0)
                        {
                            branch_code = null;
                        }

                        List<PROJECT_PARCEL_47> projectParcel47 = tdaEntities.PROJECT_PARCEL_47.Where(x => x.SUBJECT_ID.Equals(c.SUBJECT_ID) && (amphur_code == null || x.AMPHUR_CODE.Equals(amphur_code)) && (branch_code == null || x.BRANCH_CODE.Equals(branch_code))).ToList();
                        List<PROJECT_PARCEL_48> projectParcel48 = tdaEntities.PROJECT_PARCEL_48.Where(x => x.SUBJECT_ID.Equals(c.SUBJECT_ID) && (amphur_code == null || x.AMPHUR_CODE.Equals(amphur_code)) && (branch_code == null || x.BRANCH_CODE.Equals(branch_code))).ToList();

                        List<PARCEL_ViewModel> resultParcel = new List<PARCEL_ViewModel>();

                        if (projectParcel47.Count() == 0 && projectParcel48.Count() == 0)
                        {
                            //continue;
                        }

                        foreach (var projectParcel in projectParcel47)
                        {
                            var bufferQuery = $@"SELECT 
                            [OBJECTID],[OGR_FID],[PARCEL_TYPE],[UTMMAP1],[UTMMAP2],[UTMMAP3],[UTMMAP4],[UTMSCALE],[LAND_NO],[LAND_TH],[LAND_NAME],[ACTION_STATUS],[LAND_AREA],[BRANCH_CODE],[BRANCH_NAME],[CHANGWAT_CODE],[CHANGWAT_NAME],[AMPHUR_CODE],[AMPHUR_NAME],[TUMBON_CODE],[TUMBON_NAME],[CHANOD_NO],[SURVEY_NO],[TABLE_3_SEQ],[ACCOUNTING_PERIOD],[PARCEL_SHAPE],[PARCEL_RN],[STREET_RN],[BLOCK_ZONE_RN],[BLOCK_PRICE_RN],[BLOCK_FIX_RN],[BLOCK_BLUE_RN],[PREV_EVAPRICE],[CURR_EVAPRICE],[DATE_CREATED],[USER_CREATED],[DATE_UPDATED],[USER_UPDATED],[SHAPE],[GDB_GEOMATTR_DATA],[EDIT_FLAG]
                            FROM [TDASSET].[tdadmin].[PARCEL_47_{projectParcel.CHANGWAT_CODE}]
                            WHERE [BRANCH_CODE]='{projectParcel.BRANCH_CODE}' AND [PARCEL_RN]={projectParcel.PARCEL_RN} ";
                            //if (amphur_code != null && amphur_code.Length > 0)
                            //bufferQuery = $@"{bufferQuery} AND [AMPHUR_CODE] = '{amphur_code}'";
                            bufferQuery = $@"IF (EXISTS (SELECT * 
                             FROM INFORMATION_SCHEMA.TABLES 
                             WHERE TABLE_SCHEMA = 'tdadmin' 
                             AND TABLE_NAME = 'PARCEL_47_{projectParcel.CHANGWAT_CODE}'))
                             BEGIN
                              {bufferQuery}
                             END";
                            resultParcel.AddRange(tdaEntities.Database.SqlQuery<PARCEL_ViewModel>(bufferQuery).ToList());
                        }
                        foreach (var projectParcel in projectParcel48)
                        {
                            var bufferQuery = $@"SELECT 
                            [OBJECTID],[OGR_FID],[PARCEL_TYPE],[UTMMAP1],[UTMMAP2],[UTMMAP3],[UTMMAP4],[UTMSCALE],[LAND_NO],[LAND_TH],[LAND_NAME],[ACTION_STATUS],[LAND_AREA],[BRANCH_CODE],[BRANCH_NAME],[CHANGWAT_CODE],[CHANGWAT_NAME],[AMPHUR_CODE],[AMPHUR_NAME],[TUMBON_CODE],[TUMBON_NAME],[CHANOD_NO],[SURVEY_NO],[TABLE_3_SEQ],[ACCOUNTING_PERIOD],[PARCEL_SHAPE],[PARCEL_RN],[STREET_RN],[BLOCK_ZONE_RN],[BLOCK_PRICE_RN],[BLOCK_FIX_RN],[BLOCK_BLUE_RN],[PREV_EVAPRICE],[CURR_EVAPRICE],[DATE_CREATED],[USER_CREATED],[DATE_UPDATED],[USER_UPDATED],[SHAPE],[GDB_GEOMATTR_DATA],[EDIT_FLAG]
                            FROM [TDASSET].[tdadmin].[PARCEL_48_{projectParcel.CHANGWAT_CODE}]
                            WHERE [BRANCH_CODE]={projectParcel.BRANCH_CODE} AND [PARCEL_RN]={projectParcel.PARCEL_RN}";
                            //if (amphur_code != null && amphur_code.Length > 0)
                            //bufferQuery = $@"{bufferQuery} AND [AMPHUR_CODE] = '{amphur_code}'";
                            bufferQuery = $@"IF (EXISTS (SELECT * 
                             FROM INFORMATION_SCHEMA.TABLES 
                             WHERE TABLE_SCHEMA = 'tdadmin' 
                             AND  TABLE_NAME = 'PARCEL_47_{projectParcel.CHANGWAT_CODE}'))
                             BEGIN
                              {bufferQuery}
                             END";
                            resultParcel.AddRange(tdaEntities.Database.SqlQuery<PARCEL_ViewModel>(bufferQuery).ToList());
                        }



                        DbGeometry PARCEL_SHAPE = null;
                        DbGeometry PARCEL_IMPACT_SHAPE = null;
                        int parcelImpactCount = 0;

                        if (resultParcel.Count() > 0)
                        {
                            PARCEL_SHAPE = resultParcel.ElementAt(0).SHAPE;
                            if (PROJECT_AREA_SHAPE != null && PROJECT_AREA_SHAPE.Intersects(PARCEL_SHAPE))
                            {
                                PARCEL_IMPACT_SHAPE = PARCEL_SHAPE;
                                parcelImpactCount = 1;
                            }
                        }
                        for (var i = 1; i < resultParcel.Count(); i++)
                        {
                            var SHAPE = resultParcel.ElementAt(i).SHAPE;

                            if (PROJECT_AREA_SHAPE != null && PROJECT_AREA_SHAPE.Intersects(SHAPE))
                            {
                                var temp2 = PARCEL_IMPACT_SHAPE == null ? SHAPE : PARCEL_IMPACT_SHAPE.Union(SHAPE);
                                if (temp2 != null)
                                {
                                    PARCEL_IMPACT_SHAPE = temp2;
                                    parcelImpactCount++;
                                }
                            }

                            var temp = PARCEL_SHAPE.Union(SHAPE);
                            if (temp != null)
                            {
                                PARCEL_SHAPE = temp;
                            }
                        }

                        /////////////////////////////////

                        dynamic project = new
                        {
                            c.ID,
                            c.SUBJECT_ID,
                            c.SUBJECT_NAME,
                            c.PUBLISH_DATE,
                            c.CREATE_DATE,
                            c.CREATE_BY,
                            c.UPDATE_DATE,
                            c.UPDATE_BY,
                            c.PROVINCE,
                            c.IS_PUBLISHED,
                            STATUS = tdmEntities.STATUS_IMPACT.Where(y => y.ID == statusId).First(),
                            PARCEL_COUNT = resultParcel.Count,
                            PARCEL_PRICE = '-',
                            PROJECT_AREA,
                            PARCEL_SHAPE = PARCEL_SHAPE != null ? PARCEL_SHAPE.ToString() : null,
                            PROJECT_AREA_SHAPE = PROJECT_AREA_SHAPE != null ? PROJECT_AREA_SHAPE.ToString() : null,
                            PARCEL_AREA = resultParcel.Select(x => new SHAPE_ViewModel(x.SHAPE)).ToList(),
                            PARCEL_IMPACT_SHAPE = PARCEL_IMPACT_SHAPE != null ? PARCEL_IMPACT_SHAPE.ToString() : null,
                            PARCEL_IMPACT_COUNT = parcelImpactCount
                        };
                        result.Add(project);
                    }
                }

                result = result.Skip(start).Take(count).ToList();
                return Json(result, jsonSetting);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllStatus()
        {
            try
            {
                return Json(tdmEntities.STATUS_IMPACT.Select(x => new
                {
                    x.ID,
                    x.STATUS_NAME
                }).ToList());
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
    }

}
