﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace N.EntityFrameworkCore.Extensions
{
    public class TableMapping
    {
        public DbContext DbContext { get; private set; }
        public IEntityType EntityType { get; set; }
        public IProperty[] Properties { get; }
        public List<ColumnMetaData> Columns { get; set; }
        public string Schema { get; }
        public string TableName { get; }
        public StoreObjectIdentifier StoreObjectIdentifier => StoreObjectIdentifier.Table(TableName, EntityType.GetSchema());
        public string FullQualifedTableName
        {
            get { return string.Format("[{0}].[{1}]", this.Schema, this.TableName);  }
        }

        public TableMapping(DbContext dbContext, IEntityType entityType)
        {
            DbContext = dbContext;
            EntityType = entityType;
            Properties = entityType.GetProperties().ToArray();
            Schema = entityType.GetSchema() ?? "dbo";
            TableName = entityType.GetTableName();

            var options = dbContext.GetPrivateFieldValue("_options") as DbContextOptions;
        }
        public IEnumerable<string> GetColumns(bool includeAutoGenerated=false)
        {
            return EntityType.GetProperties().Where(o => includeAutoGenerated || o.ValueGenerated == ValueGenerated.Never)
                .Select(o => o.GetColumnName(this.StoreObjectIdentifier));
        }
        public IEnumerable<string> GetPrimaryKeyColumns()
        {
            return EntityType.FindPrimaryKey().Properties.Select(o => o.GetColumnName(this.StoreObjectIdentifier));
        }
    }
    public class ColumnMetaData
    {
        public EFColumn2 Column { get; internal set; }
        public EFColumnProperty Property { get; internal set; }
    }

    public class EFColumn2
    {
        public bool IsStoreGeneratedIdentity { get; internal set; }
        public string Name { get; internal set; }
    }

    public class EFColumnProperty
    {
        public string Name { get; internal set; }
    }
}

