using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Localization.SqlLocalizer;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace IdentityServer.CustomServices
{
    public class MySqlStringLocalizerFactory : SqlStringLocalizerFactory, IStringLocalizerFactory
    {
        public MySqlStringLocalizerFactory(LocalizationModelContext context, DevelopmentSetup developmentSetup,
            IOptions<SqlLocalizationOptions> localizationOptions) : base(context, developmentSetup, localizationOptions)
        {
        }

        public new IStringLocalizer Create(string baseName, string location)
        {
            FieldInfo fieldInfo = typeof(SqlStringLocalizerFactory).GetField("_resourceLocalizations",
                BindingFlags.Static | BindingFlags.NonPublic);
            var resourceLocalizations = (ConcurrentDictionary<string, IStringLocalizer>)fieldInfo.GetValue(this);

            fieldInfo = typeof(SqlStringLocalizerFactory).GetField("_developmentSetup",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var developmentSetup = (DevelopmentSetup)fieldInfo.GetValue(this);

            fieldInfo = typeof(SqlStringLocalizerFactory).GetField("_options",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var localizationOptions = (IOptions<SqlLocalizationOptions>)fieldInfo.GetValue(this);

            if (resourceLocalizations.Keys.Contains(baseName + location))
                return resourceLocalizations[baseName + location];

            Type type = GetType().BaseType;
            MethodInfo method = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(m => m.Name == "GetAllFromDatabaseForResource");
            SqlStringLocalizerFactory baseFactory = this;
            Dictionary<string, string> localizations =
                (Dictionary<string, string>)method.Invoke(baseFactory, new object[] {baseName + location});

            SqlStringLocalizer sqlStringLocalizer = new SqlStringLocalizer(localizations, developmentSetup, baseName + location, localizationOptions.Value.ReturnOnlyKeyIfNotFound, localizationOptions.Value.CreateNewRecordWhenLocalisedStringDoesNotExist);
            return resourceLocalizations.GetOrAdd(baseName + location, sqlStringLocalizer);
        }
    }
}
