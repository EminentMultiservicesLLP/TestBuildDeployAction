using System.Web;
using System.Web.Optimization;

namespace CGHSBilling
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.min.js",
                       "~/Scripts/jquery-ui-{version}.min.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryMore").Include(
                       "~/Scripts/moment.min.js",
                       "~/Scripts/bootstrap.min.js",
                       "~/Scripts/bootstrap-datetimepicker.js",
                       "~/Scripts/jquery.timepicker.min.js",
                       "~/Scripts/jLinq-2.2.1.js",
                       "~/Scripts/common.js",
                       "~/Scripts/prefixMenu.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymodels").Include(
                       "~/Scripts/SupplierModel.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery/external").Include(
                        "~/Scripts/pqgrid/pqgrid.min.js",
                        //"~/Scripts/pqgrid/pqselect.min.js",
                        "~/Scripts/MessageBox/lobibox.min.js",
                        "~/Scripts/MessageBox/messageboxes.min.js",
                        "~/Scripts/MessageBox/notifications.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new StyleBundle("~/Content/cssMust").Include("~/Content/bootstrap.css",
                "~/Content/font-awesome/css/font-awesome.min.css",
                "~/Content/bootstrap-datetimepicker.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css",
                "~/Content/MenuStyle.css"
                ));


            bundles.Add(new StyleBundle("~/Content/external").Include(
               "~/Content/pqgrid/pqgrid.min.css",
               "~/Content/pqgrid/pqgrid.ui.min.css",
               "~/Content/pqgrid/pqgrid.bootstrap.min.css",
               "~/Content/pqgrid/themes/chocolate/pqgrid.css",
               "~/Content/MessageBox/lobibox.min.css"
                ));


            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/core.css",
                        "~/Content/themes/base/resizable.css",
                        "~/Content/themes/base/selectable.css",
                        "~/Content/themes/base/accordion.css",
                        "~/Content/themes/base/autocomplete.css",
                        "~/Content/themes/base/button.css",
                        "~/Content/themes/base/dialog.css",
                        "~/Content/themes/base/slider.css",
                        "~/Content/themes/base/tabs.css",
                        "~/Content/themes/base/datepicker.css",
                        "~/Content/themes/base/progressbar.css",
                        "~/Content/themes/base/theme.css",
                        "~/Content/jquery.timepicker.css"));

            bundles.IgnoreList.Clear();

            //#if DEBUG
            //  BundleTable.EnableOptimizations = false;
            //#else
            //BundleTable.EnableOptimizations = true;
            //#endif

        }
    }
}