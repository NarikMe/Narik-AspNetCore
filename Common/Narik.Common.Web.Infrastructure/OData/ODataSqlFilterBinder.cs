using System;
using System.Linq;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using AllNode = Microsoft.OData.UriParser.AllNode;
using AnyNode = Microsoft.OData.UriParser.AnyNode;
using BinaryOperatorNode = Microsoft.OData.UriParser.BinaryOperatorNode;
using CollectionNavigationNode = Microsoft.OData.UriParser.CollectionNavigationNode;
using CollectionNode = Microsoft.OData.UriParser.CollectionNode;
using CollectionPropertyAccessNode = Microsoft.OData.UriParser.CollectionPropertyAccessNode;
using ConstantNode = Microsoft.OData.UriParser.ConstantNode;
using ConvertNode = Microsoft.OData.UriParser.ConvertNode;
using FilterClause = Microsoft.OData.UriParser.FilterClause;
using QueryNode = Microsoft.OData.UriParser.QueryNode;
using SingleEntityNode = Microsoft.OData.UriParser.SingleEntityNode;
using SingleNavigationNode = Microsoft.OData.UriParser.SingleNavigationNode;
using SingleValueNode = Microsoft.OData.UriParser.SingleValueNode;
using SingleValuePropertyAccessNode = Microsoft.OData.UriParser.SingleValuePropertyAccessNode;
using UnaryOperatorNode = Microsoft.OData.UriParser.UnaryOperatorNode;

namespace Narik.Common.Web.Infrastructure.OData
{
    public class ODataSqlFilterBinder
    {
        private IEdmModel _model;
        private bool _nonApplyString = false;
        protected ODataSqlFilterBinder(IEdmModel model)
        {
            _model = model;
        }

        public static string BindFilterQueryOption(FilterQueryOption filterQuery)
        {
            if (filterQuery != null)
            {
                ODataSqlFilterBinder binder = new ODataSqlFilterBinder(filterQuery.Context.Model);
                return  binder.BindFilter(filterQuery) + Environment.NewLine;
            }

            return "";
        }

        protected string BindFilter(FilterQueryOption filterQuery)
        {
            return BindFilterClause(filterQuery.FilterClause);
        }

        protected string BindFilterClause(FilterClause filterClause)
        {
            return Bind(filterClause.Expression);
        }

        protected string Bind(QueryNode node)
        {
            CollectionNode collectionNode = node as CollectionNode;
            SingleValueNode singleValueNode = node as SingleValueNode;

            if (collectionNode != null)
            {
                switch (node.Kind)
                {
                    case QueryNodeKind.CollectionNavigationNode:
                        CollectionNavigationNode navigationNode = node as CollectionNavigationNode;
                        return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty);

                    case QueryNodeKind.CollectionPropertyAccess:
                        return BindCollectionPropertyAccessNode(node as CollectionPropertyAccessNode);
                }
            }
            else if (singleValueNode != null)
            {
                switch (node.Kind)
                {
                    case QueryNodeKind.BinaryOperator:
                        return BindBinaryOperatorNode(node as BinaryOperatorNode);

                    case QueryNodeKind.Constant:
                        return BindConstantNode(node as ConstantNode);

                    case QueryNodeKind.Convert:
                        return BindConvertNode(node as ConvertNode);

                    //case QueryNodeKind.EntityRangeVariableReference:
                    //    return BindRangeVariable((node as EntityRangeVariableReferenceNode).RangeVariable);

                    //case QueryNodeKind.NonentityRangeVariableReference:
                    //    return BindRangeVariable((node as NonentityRangeVariableReferenceNode).RangeVariable);

                    case QueryNodeKind.SingleValuePropertyAccess:
                        return BindPropertyAccessQueryNode(node as SingleValuePropertyAccessNode);

                    case QueryNodeKind.UnaryOperator:
                        return BindUnaryOperatorNode(node as UnaryOperatorNode);

                    case QueryNodeKind.SingleValueFunctionCall:
                        return BindSingleValueFunctionCallNode(node as SingleValueFunctionCallNode);

                    case QueryNodeKind.SingleNavigationNode:
                        SingleNavigationNode navigationNode = node as SingleNavigationNode;
                        return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty);

                    case QueryNodeKind.Any:
                        return BindAnyNode(node as AnyNode);

                    case QueryNodeKind.All:
                        return BindAllNode(node as AllNode);
                }
            }

            throw new NotSupportedException(String.Format("Nodes of type {0} are not supported", node.Kind));
        }

        private string BindCollectionPropertyAccessNode(CollectionPropertyAccessNode collectionPropertyAccessNode)
        {
            return Bind(collectionPropertyAccessNode.Source) + "." + collectionPropertyAccessNode.Property.Name;
        }

        private string BindNavigationPropertyNode(SingleValueNode singleValueNode, IEdmNavigationProperty edmNavigationProperty)
        {
            return Bind(singleValueNode) + "." + edmNavigationProperty.Name;
        }

        private string BindAllNode(AllNode allNode)
        {
            string innerQuery = "not exists ( from " + Bind(allNode.Source) + " " + allNode.RangeVariables.First().Name;
            innerQuery += " where NOT(" + Bind(allNode.Body) + ")";
            return innerQuery + ")";
        }

        private string BindAnyNode(AnyNode anyNode)
        {
            string innerQuery = "exists ( from " + Bind(anyNode.Source) + " " + anyNode.RangeVariables.First().Name;
            if (anyNode.Body != null)
            {
                innerQuery += " where " + Bind(anyNode.Body);
            }
            return innerQuery + ")";
        }

        private string BindNavigationPropertyNode(SingleEntityNode singleEntityNode, IEdmNavigationProperty edmNavigationProperty)
        {
            return Bind(singleEntityNode) + "." + edmNavigationProperty.Name;
        }

        private string BindSingleValueFunctionCallNode(SingleValueFunctionCallNode singleValueFunctionCallNode)
        {
            var arguments = singleValueFunctionCallNode.Parameters.ToList();
            switch (singleValueFunctionCallNode.Name)
            {
                case "contains":
                    _nonApplyString = true;
                    var ret= Bind(arguments[0]) +" like N'%"+   Bind(arguments[1]) + "%'";
                    _nonApplyString = false;
                    return ret;
                case "indexof":
                    _nonApplyString = true;
                    var ret5 =  Bind(arguments[0]) + " Not like N'%" + Bind(arguments[1]) + "%'";
                    _nonApplyString = false;
                    return ret5;
                case "startswith":
                    _nonApplyString = true;
                    var ret2 = Bind(arguments[0]) + " like N'" + Bind(arguments[1]) + "%'";
                    _nonApplyString = false;
                    return ret2;
                case "endswith":
                    _nonApplyString = true;
                    var ret3 = Bind(arguments[0]) + " like N'%" + Bind(arguments[1]) + "'";
                    _nonApplyString = false;
                    return ret3;
                case "concat":
                    return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0]) + "," + Bind(arguments[1]) + ")";

                case "length":
                case "trim":
                case "year":
                case "years":
                case "month":
                case "months":
                case "day":
                case "days":
                case "hour":
                case "hours":
                case "minute":
                case "minutes":
                case "second":
                case "seconds":
                case "round":
                case "floor":
                case "ceiling":
                    return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0]) + ")";
                default:
                    throw new NotImplementedException();
            }
        }

        private string BindUnaryOperatorNode(UnaryOperatorNode unaryOperatorNode)
        {
            return ToString(unaryOperatorNode.OperatorKind) + "(" + Bind(unaryOperatorNode.Operand) + ")";
        }

        private string BindPropertyAccessQueryNode(SingleValuePropertyAccessNode singleValuePropertyAccessNode)
        {
            return singleValuePropertyAccessNode.Property.Name;
        }

        private string BindConvertNode(ConvertNode convertNode)
        {
            return Bind(convertNode.Source);
        }

        private string BindConstantNode(ConstantNode constantNode)
        {
            if (constantNode.Value is string && !_nonApplyString)
            {
                return String.Format("N'{0}'", constantNode.Value);
            }
            if (constantNode.Value == null)
                return " Null ";
            if (constantNode.Value is Boolean)
                return Convert.ToInt32(constantNode.Value).ToString();
            return constantNode.Value.ToString();
        }

        private string BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode)
        {
            var left = Bind(binaryOperatorNode.Left);
            var node = binaryOperatorNode.Left as SingleValueFunctionCallNode;
            if (node != null &&
                node.Name=="indexof")
            {
                return "(" + left + ")";
            }
            var right = Bind(binaryOperatorNode.Right);
            if (right != null && right.Trim() == " Null ".Trim() && (binaryOperatorNode.Right is ConvertNode))
            {
                if (((binaryOperatorNode.Right as ConvertNode).Source as ConstantNode).Value==null)
                    return "(" + left + (binaryOperatorNode.OperatorKind==BinaryOperatorKind.Equal ? " Is " : " Is not ") + " " + right + ")";
            }
            return "(" + left + " " + ToString(binaryOperatorNode.OperatorKind) + " " + right + ")";
        }

        private string ToString(BinaryOperatorKind binaryOpertor)
        {
            switch (binaryOpertor)
            {
                case BinaryOperatorKind.Add:
                    return "+";
                case BinaryOperatorKind.And:
                    return "AND";
                case BinaryOperatorKind.Divide:
                    return "/";
                case BinaryOperatorKind.Equal:
                    return "=";
                case BinaryOperatorKind.GreaterThan:
                    return ">";
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return ">=";
                case BinaryOperatorKind.LessThan:
                    return "<";
                case BinaryOperatorKind.LessThanOrEqual:
                    return "<=";
                case BinaryOperatorKind.Modulo:
                    return "%";
                case BinaryOperatorKind.Multiply:
                    return "*";
                case BinaryOperatorKind.NotEqual:
                    return "!=";
                case BinaryOperatorKind.Or:
                    return "OR";
                case BinaryOperatorKind.Subtract:
                    return "-";
                default:
                    return null;
            }
        }

        private string ToString(UnaryOperatorKind unaryOperator)
        {
            switch (unaryOperator)
            {
                case UnaryOperatorKind.Negate:
                    return "!";
                case UnaryOperatorKind.Not:
                    return "NOT";
                default:
                    return null;
            }
        }
    }
}
