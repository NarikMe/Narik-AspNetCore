using System;
using System.Collections.Generic;
using Microsoft.AspNet.OData;
using Narik.Common.Shared.Attributes;
using Narik.Common.Shared.Constants;
using Narik.Common.Shared.Interfaces;

namespace Narik.Common.Shared.Models
{
    public class NarikApiActionDescriptor
    {
        public NarikApiActionDescriptor(string controlName, string actionName,string engName=null)
        {
            if (string.IsNullOrEmpty(engName))
                EngName = controlName.Replace("Controller", "");
            else
                EngName = engName;
            switch (actionName)
            {
                case "DeleteEntity":
                    CheckWithCreator = false;
                    Action = "Delete";
                    break;
                case "Get":  
                    CheckWithCreator = false;
                    Action = "";
                    break;
                case "Post":
                    Action = null;
                   // FuncAction = PostDefaultFuncAction();
                    break;
                case "PostMasterDetail":
                    Action = null;
                   // FuncAction = PostMasterDefaultFuncAction();
                    break;
                case "PostBatch":
                    Action = null;
                    FuncAction = args => "Edit";
                    FuncCheckWithCreator = args => false;
                    FuncGetCreators = args =>
                    {
                        return new[] { 0 };
                    };
                    break;
            }
            if (actionName != "Get" && actionName.StartsWith("Get"))
            {
                CheckWithCreator = false;
                Action = "";
            }
        }

        public NarikApiActionDescriptor(NarikApiActionAttribute actionAtt)
        {
            IWebApiActionMetaData metaData=null;
            if (actionAtt.MetadDataType!=null)
                 metaData = Activator.CreateInstance(actionAtt.MetadDataType) as IWebApiActionMetaData;
            EngName = actionAtt.EngName;
            Action = actionAtt.Action;
            NoCheckAccesslevel = actionAtt.NoCheckAccesslevel;
            if (metaData != null)
            {
                FuncGetCreators = metaData.FuncGetCreators;
                FuncCheckWithCreator = metaData.FuncCheckWithCreator;
                FuncAction = metaData.FuncAction;
                FuncEngName = metaData.FuncEngName;
                CheckWithCreator = actionAtt.CheckWithCreator;
            }

        }

        #region static members

      

       

       
        //public static Func<Dictionary<string, object>, string> PostDefaultFuncAction()
        //{
        //    return args =>
        //    {
        //        var saturnViewModel = args["entity"] as INarikViewModel;
        //        return saturnViewModel != null && saturnViewModel.ViewModelId != NarikConstant.NewId ? "Edit" : "New";
        //    };
        //}

        

     

        //public static Func<Dictionary<string, object>, string> PostMasterDefaultFuncAction()
        //{
        //    return args =>
        //    {
        //        var parameters = args["parameters"] as ODataActionParameters;
        //        if (parameters==null || !parameters.ContainsKey("entity"))
        //            return "New";
        //        var saturnViewModel = parameters["entity"] as INarikViewModel;
        //        return saturnViewModel != null && saturnViewModel.ViewModelId != NarikConstant.NewId ? "Edit" : "New";
        //    };
        //}

        #endregion

        public string EngName { get; set; }
        public string  Action { get; set; }
        public bool NoCheckAccesslevel { get; set; }
        public Func<Dictionary<string, object>, int[]> FuncGetCreators { get; private set; }
        public Func<Dictionary<string, object>, bool> FuncCheckWithCreator { get; private set; }
        public Func<Dictionary<string, object>, string> FuncAction { get; private set; }
        public Func<Dictionary<string, object>, string> FuncEngName { get; private set; }
        public bool? CheckWithCreator { get; set; }
    }
}
