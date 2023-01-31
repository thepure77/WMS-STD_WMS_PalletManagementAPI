using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using DataAccess;
using DataAccess.Models.Master.Table;
using DataAccess.Models.Master.View;
using DataAccess.Models.Transfer.Table;

using Business.Commons;
using Business.Extensions;
using PalletmanageBusiness.ModelConfig;
using System.Data.SqlClient;
using Comone.Utils;
using DataAccess.Models.Transfer.StoredProcedure;
using System.Threading;
using BinBalanceDataAccess.Models;

namespace Business.Services
{
    public static class PalletmanageExtensions
    {
        public static DateTime TrimTime(this DateTime data)
        {
            return DateTime.Parse(data.ToShortDateString());
        }

        public static bool IsEquals(this Guid field, Guid? condition)
        {
            return condition.HasValue ? field.Equals(condition) : true;
        }

        public static bool IsEquals(this int field, int? condition)
        {
            return condition.HasValue ? field.Equals(condition) : true;
        }

        public static bool IsEquals(this bool field, bool? condition)
        {
            return condition.HasValue ? field.Equals(condition) : true;
        }

        public static bool Like(this string field, string condition)
        {
            return condition != null ? (field?.Contains(condition) ?? false) : true;
        }

        public static bool DateBetweenField(this DateTime? field_from, DateTime? field_to, DateTime? condition)
        {
            return condition.HasValue ? (field_from.HasValue ? (field_to.HasValue ? condition >= field_from && condition <= field_to : field_from.Equals(condition)) : false) : true;
        }

        public static bool DateBetweenCondition(this DateTime field, DateTime? condition_from, DateTime? condition_to, bool trimTime = false)
        {
            return condition_from.HasValue ? (condition_to.HasValue ? condition_from <= (trimTime ? field.TrimTime() : field) && condition_to >= (trimTime ? field.TrimTime() : field) : (trimTime ? field.TrimTime() : field).Equals(condition_from)) : true;
        }

        public static bool DateBetweenCondition(this DateTime? field, DateTime? condition_from, DateTime? condition_to)
        {
            return condition_from.HasValue ? (field.HasValue ? (condition_to.HasValue ? condition_from <= field && condition_to >= field : field.Equals(condition_from)) : false) : true;
        }
    }

}
