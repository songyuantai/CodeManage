using CodeManager.Busi;
using CodeManager.Busi.Gen;
using CodeManager.Core;
using CodeManager.Model;
using CodeManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodeManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BllSysDb _sysDb;

        public HomeController(ILogger<HomeController> logger, BllSysDb sysDb)
        {
            _logger = logger;
            _sysDb = sysDb;
        }

        public IActionResult Index()
        {
            var list = _sysDb.GetList<DbConnection>();
            return View(list);
        }

        public IActionResult ConnectionAdd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConnectionAdd(DbConnection model)
        {
            model.Id = App.NewGuid();
            ViewBag.success = _sysDb.Add(model);
            return View(model);
        }

        public IActionResult ConnectionEdit(string id)
        {
            var model = _sysDb.GetModel<DbConnection>(id);
            return View(model);
        }

        

        [HttpPost]
        public IActionResult ConnectionEdit(DbConnection model)
        {
            ViewBag.success = _sysDb.Update(model);
            return View(model);
        }

        public IActionResult ConnectionDetail(string id)
        {
            var list = _sysDb.GetConnectTableList(id);
            ViewBag.id = id;
            return View(list);
        }

        public IActionResult MergeAllTables(string id)
        {
            var connection = _sysDb.GetModel<DbConnection>(id);
            var bllOracle = new BllSysOracle(connection.ConnectString);
            bllOracle.Merge(_sysDb, connection);
            return Json(true);
        }

        public IActionResult Build(BuildViewModel model)
        {
            var tables = _sysDb.GetList<DbTable>(model.TableIds);
            var columns = _sysDb.GetTableColumnList(model.TableIds.ToArray());

            //创建生成目录
            if (!Directory.Exists(model.OutputPath))
            {
                Directory.CreateDirectory(model.OutputPath);
            }

            foreach(var table in tables)
            {
                var tableColumns = columns.Where(m => m.TableId == table.Id).ToList();
                var param = new BuilderParam
                {
                    NameSpaceName = model.NameSpace,
                    Table = table,
                    Columns = tableColumns
                };
                string text;

                if (model.Template == "simple")
                {
                    var builder = new SimpleModelBuilder(param);
                    text = builder.TransformText();
                }
                else if (model.Template == "model")
                {
                    var builder = new ModelBuilder(param);
                    text = builder.TransformText();
                } else if(model.Template == "xml")
                {
                    var builder = new XmlBuilder(param);
                    text = builder.TransformText();
                }
                else
                {
                    text = string.Empty;
                }

                var fileName = Tool.PascalToCamel(table.TableName) + ".cs";
                var filePath = Path.Combine(model.OutputPath, fileName);
                System.IO.File.WriteAllText(filePath, text);
            }

            return Json(true);
        }

        public IActionResult ConnectionDel(string id)
        {
            _sysDb.DeleteConnections(id);
            return Json(true);
        }

        public IActionResult TableDel(string id)
        {
            _sysDb.DeleteTables(id);
            return Json(true);
        }

        public IActionResult TableDetail(string id)
        {
            var list = _sysDb.GetTableColumnList(id);
            return View(list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}