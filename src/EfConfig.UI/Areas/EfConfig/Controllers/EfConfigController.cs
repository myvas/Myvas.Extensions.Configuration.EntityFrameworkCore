using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Myvas.Extensions.Configuration.EntityFrameworkCore.Models;
using Myvas.Extensions.Configuration.EntityFrameworkCore.UI;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore.UI.Controllers
{
    [Area("EfConfig")]
    [Authorize(Policy = "AdminEfConfig")]
    public class EfConfigController : Controller
    {
        private readonly ConfigDbContext _config;
        private readonly ILogger _logger;

        public EfConfigController(ConfigDbContext config,
            ILoggerFactory loggerFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = loggerFactory?.CreateLogger<EfConfigController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task<IActionResult> Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                var vm = await _config.ConfigurationValues
                    .AsNoTracking()
                    .ToListAsync();
                return View(vm);
            }
            else
            {
                ViewData["q"] = q;
                var vm = await _config.ConfigurationValues
                    .AsNoTracking()
                    .Where(x => string.Concat(x.Key, " ", x.Value).ToLower().Contains(q.ToLower()))
                    .ToListAsync();
                return View(vm);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Key,Value")] ConfigurationValue vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var item = new ConfigurationValue()
            {
                Key = vm.Key,
                Value = vm.Value
            };

            await _config.ConfigurationValues.AddAsync(item);

            try
            {
                var saveResult = await _config.SaveChangesAsync();
                if (saveResult < 1)
                {
                    AddError("未能成功存入数据库");
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "存储参数时发生异常！");
                throw ex;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Key,Value")] ConfigurationValue vm)
        {
            if (id != vm.Key)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var item = new ConfigurationValue()
            {
                Key = vm.Key,
                Value = vm.Value
            };

            _config.ConfigurationValues.Update(item);

            try
            {
                await _config.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "存储参数时发生异常！");
                throw ex;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPut("api/config/value")]
        public async Task<IActionResult> PutConfigValue([FromBody]KeyValueViewModel kv)
        {
            var item = new ConfigurationValue()
            {
                Key = kv.Key,
                Value = kv.Value
            };
            var exists = await _config.ConfigurationValues
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Key == kv.Key);
            if (exists == null)
            {
                _config.ConfigurationValues.Add(item);
            }
            else
            {
                _config.ConfigurationValues.Update(item);
            }

            try
            {
                var result = await _config.SaveChangesAsync();
                if (result > 0)
                {
                    return Json(EfConfigJson.Ok());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "存储配置数据时发生异常！");
            }
            return Json(new {ErrorMessage = "数据更新失败" });
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var data = await _config.ConfigurationValues
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Key == id);
            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var data = await _config.ConfigurationValues.FirstOrDefaultAsync(x => x.Key == id);
            if (data == null)
            {
                return NotFound();
            }
            _config.ConfigurationValues.Remove(data);

            try
            {
                await _config.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "存储配置数据时发生异常！");
                throw ex;
            }

            return RedirectToAction(nameof(Index));
        }


        #region Helpers
        protected void AddError(string error)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        #endregion
    }
}