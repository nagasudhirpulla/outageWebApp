using Oracle.ManagedDataAccess.Client;
using OutageDataLayer.QueryModels;
using OutageDataLayer.QueryResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutageDataLayer.DataLayer
{
    public class OutageFetcher
    {
        public List<OutageQueryResult> FetchOutages(OutageQuery outageQuery, string connStr)
        {
            QueryExecuter queryExecuter = new QueryExecuter { ConnStr = connStr };
            /*
             SELECT E.ELEMENT_NAME,O.* FROM OUTAGES O 
            LEFT OUTER JOIN ELEMENTS E ON O.ELEMENT_ID=E.ELEMENT_ID 
            WHERE 
            O.DEV_TYPE='R' AND
            TRUNC(O.OUT_DATE)=TO_DATE('21-SEP-18','dd-MON-yy')
            order by O.OUT_DATE desc;
            */
            string mainQueryPrefix = "SELECT COALESCE(E.VOL_RATING,'NA') AS VOL_RATING, E.ELEMENT_NAME, E.OWNER_NAME, O.*, SD_REASONS.REASON FROM OUTAGES O LEFT OUTER JOIN(SELECT ELEMENTS.*, OWNERS.NAME AS OWNER_NAME FROM ELEMENTS LEFT OUTER JOIN OWNERS ON OWNERS.ID = ELEMENTS.OWNER_ID) E ON O.ELEMENT_ID = E.ELEMENT_ID LEFT OUTER JOIN SD_REASONS ON SD_REASONS.REASON_ID = O.REASON_ID ";
            //examine the query params
            List<string> whereStrings = new List<string>();
            List<OracleParameter> whereParams = new List<OracleParameter>();

            // hangle element id
            if (outageQuery.ElementId != null)
            {
                whereStrings.Add("O.ELEMENT_ID = :element_id");
                whereParams.Add(new OracleParameter("element_id", outageQuery.ElementId));
            }

            // hangle element type
            if (outageQuery.ElementType != null)
            {
                whereStrings.Add("O.DEV_TYPE = :element_type");
                whereParams.Add(new OracleParameter("element_type", outageQuery.ElementType));
            }

            // hangle outage type
            if (outageQuery.ReasonSearchText != null)
            {
                whereStrings.Add("UPPER(O.COMMENTS) LIKE :comment_search");
                whereParams.Add(new OracleParameter("comment_search", "%" + outageQuery.ReasonSearchText.ToUpper() + "%"));
            }

            // hangle reason text
            if (outageQuery.OutageType != null)
            {
                whereStrings.Add("O.REASON_TYPE = :outage_type");
                whereParams.Add(new OracleParameter("outage_type", outageQuery.OutageType));
            }

            // hangle in from date
            if (outageQuery.InFromDate != null)
            {
                DateTime inFromTime = outageQuery.InFromDate.Value;
                whereStrings.Add("( TRUNC(IN_DATE) >= TO_DATE(:in_from_date,'YYYY/MM/DD') )");
                whereParams.Add(new OracleParameter("in_from_date", inFromTime.ToString("yyyy/MM/dd")));
            }

            // hangle in to date
            if (outageQuery.InToDate != null)
            {
                DateTime inToTime = outageQuery.InToDate.Value;
                whereStrings.Add("( TRUNC(IN_DATE) <= TO_DATE(:in_to_date,'YYYY/MM/DD') )");
                whereParams.Add(new OracleParameter("in_to_date", inToTime.ToString("yyyy/MM/dd")));
            }

            // hangle out from date
            // if no out out from time is given, the default out from time would be yesterday, we are doing to avoid huge data fetches unnecessarily
            DateTime outFromTime = DateTime.Now.AddDays(-1);
            if (outageQuery.OutFromDate != null)
            {
                outFromTime = outageQuery.OutFromDate.Value;
            }
            whereStrings.Add("( TRUNC(OUT_DATE) >= TO_DATE(:out_from_date,'YYYY/MM/DD') )");
            whereParams.Add(new OracleParameter("out_from_date", outFromTime.ToString("yyyy/MM/dd")));

            // hangle out to date
            if (outageQuery.OutToDate != null)
            {
                DateTime outToTime = outageQuery.OutToDate.Value;
                whereStrings.Add("( TRUNC(OUT_DATE) <= TO_DATE(:out_to_date,'YYYY/MM/DD') )");
                whereParams.Add(new OracleParameter("out_to_date", outToTime.ToString("yyyy/MM/dd")));
            }            

            string whereStr = string.Join(" AND ", whereStrings);
            if (!string.IsNullOrWhiteSpace(whereStr))
            {
                whereStr = " where " + whereStr;
            }
            string mainQueryEnd = " order by O.OUT_DATE desc ";
            string mainQuery = mainQueryPrefix + whereStr + mainQueryEnd;
            TableRowsApiResultModel tableRowsResult = queryExecuter.GetDbTableRows(mainQuery, whereParams);
            // iterate through rows and return the queryresult
            List<OutageQueryResult> outageQueryResults = new List<OutageQueryResult>();
            int elementIdIndex = tableRowsResult.TableColNames.IndexOf("ELEMENT_ID");
            int elementNameIndex = tableRowsResult.TableColNames.IndexOf("ELEMENT_NAME");
            int elementTypeIndex = tableRowsResult.TableColNames.IndexOf("DEV_TYPE");
            int outageTypeIndex = tableRowsResult.TableColNames.IndexOf("REASON_TYPE");
            int outTimeIndex = tableRowsResult.TableColNames.IndexOf("OUT_DATE");
            int inTimeIndex = tableRowsResult.TableColNames.IndexOf("IN_DATE");
            int voltageLevelIndex = tableRowsResult.TableColNames.IndexOf("VOL_RATING");
            int outageReasonIndex = tableRowsResult.TableColNames.IndexOf("COMMENTS");
            int outageCategoryIndex = tableRowsResult.TableColNames.IndexOf("REASON");
            int elementOwnerIndex = tableRowsResult.TableColNames.IndexOf("OWNER_NAME");

            for (int rowIter = 0; rowIter < tableRowsResult.TableRows.Count; rowIter++)
            {
                OutageQueryResult res = new OutageQueryResult();
                if (elementIdIndex != -1)
                {
                    res.ElementId = Convert.ToInt32((decimal)tableRowsResult.TableRows[rowIter][elementIdIndex]);
                }
                if (elementNameIndex != -1)
                {
                    res.ElementName = (string)tableRowsResult.TableRows[rowIter][elementNameIndex];
                }
                if (elementOwnerIndex != -1)
                {
                    res.ElementOwner = (string)tableRowsResult.TableRows[rowIter][elementOwnerIndex];
                }
                if (elementTypeIndex != -1)
                {
                    res.ElementType = (string)tableRowsResult.TableRows[rowIter][elementTypeIndex];
                }
                if (inTimeIndex != -1)
                {
                    var inTime = tableRowsResult.TableRows[rowIter][inTimeIndex];
                    if (inTime.GetType() != typeof(System.DBNull))
                    {
                        res.InTime = (DateTime)inTime;
                    }
                }
                if (outTimeIndex != -1)
                {
                    res.OutTime = (DateTime)tableRowsResult.TableRows[rowIter][outTimeIndex];
                }
                if (voltageLevelIndex != -1)
                {
                    res.VoltageLevel = (string)tableRowsResult.TableRows[rowIter][voltageLevelIndex];
                }
                if (outageTypeIndex != -1)
                {
                    var outageType = tableRowsResult.TableRows[rowIter][outageTypeIndex];
                    if (outageType.GetType() != typeof(System.DBNull))
                    {
                        res.OutageType = (string)outageType;
                    }
                }
                if (outageReasonIndex != -1)
                {
                    var outageReason = tableRowsResult.TableRows[rowIter][outageReasonIndex];
                    if (outageReason.GetType() != typeof(System.DBNull))
                    {
                        res.OutageReason = (string)outageReason;
                    }
                }
                if (outageCategoryIndex != -1)
                {
                    var outageCategory = tableRowsResult.TableRows[rowIter][outageCategoryIndex];
                    if (outageCategory.GetType() != typeof(System.DBNull))
                    {
                        res.OutageCategory = (string)outageCategory;
                    }
                }
                outageQueryResults.Add(res);
            }

            return outageQueryResults;
        }
    }
}
