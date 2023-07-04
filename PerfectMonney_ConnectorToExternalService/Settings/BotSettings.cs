﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PerfectMonney_ConnectorToExternalService.Settings
{
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare type inside namespace.
    public class BotSettings
    {
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
        public static readonly string Configuration = "BotSettings";
        
        public bool StopSelling { get; set; } = false;
        // public bool Repair { get; set; }
        public List<string> RuleText { get; set; }
        [NotMapped]
        public string RuleTextAsOneString { get => _ruleTextAsOneString??= MakeListRuleTextToSingleText(); }
        public string AboutUs { get; set; }
        private string _ruleTextAsOneString;
        private string MakeListRuleTextToSingleText()
        {
            StringBuilder stringBuilder= new StringBuilder();
            foreach (var item in RuleText)
            {
                stringBuilder.AppendLine(item);
            }
            return stringBuilder.ToString();
        }

    }
}
