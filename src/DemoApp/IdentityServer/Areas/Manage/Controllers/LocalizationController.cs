using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Extensions;
using IdentityServer.HttpHandlerBase;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using X.PagedList;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class LocalizationController : BaseController
    {
        private readonly LocalizationModelContext _context;
        private readonly IStringExtendedLocalizerFactory _stringExtendedLocalizerFactory;
        //private readonly RazorViewToStringRenderer _razorViewToStringRenderer;
        public LocalizationController(LocalizationModelContext context, IStringExtendedLocalizerFactory stringExtendedLocalizerFactory/*, RazorViewToStringRenderer razorViewToStringRenderer*/)
        {
            _context = context;
            _stringExtendedLocalizerFactory = stringExtendedLocalizerFactory;
            //_razorViewToStringRenderer = razorViewToStringRenderer;
        }

        // GET: DomainDemo
        public async Task<IActionResult> Index(string culture, string resourceKey, string hiddenTranslatedText, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;

            List<string> resourceKeys = null;

            for (int i = 0; i <= 3; i++)
            {
                try
                {
                    resourceKeys = _context.LocalizationRecords.AsNoTracking().GroupBy(r => r.ResourceKey)
                        .Select(r => r.Key).ToList();
                    break;
                }
                catch
                {
                    if (i >= 3)
                    {
                        throw;
                    }
                    Thread.Sleep(500);
                }
            }
            
            ViewBag.ResourceKeys = resourceKeys;
            var query = _context.LocalizationRecords.AsNoTracking();
            if (hiddenTranslatedText == "on")
            {
                query = query.Where(r => r.Text == r.Key + "." + r.LocalizationCulture);
            }
            if (!culture.IsNullOrWhiteSpace())
            {
                query = query.Where(r => r.LocalizationCulture == culture);
            }

            if (!resourceKey.IsNullOrWhiteSpace())
            {
                query = query.Where(r => r.ResourceKey == resourceKey);
            }

            query = query.OrderBy(d=>d.Id);
            var model = await query.ToPagedListAsync(pageIndex, pageSize);

            //Razor视图渲染为字符串
            //var razorViewString = await _razorViewToStringRenderer.RenderViewToStringAsync("_IndexTBody1", model, ControllerContext);

            if (this.IsAjaxRequest())
            {
                return PartialView("_IndexTBody", model);
            }
            else
            {
                return View(model);
            }
        }

        // GET: DomainDemo/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            var record = await _context.LocalizationRecords.FirstOrDefaultAsync(m => m.Id == id);
            if (record == null)
            {
                return NotFoundView();
            }

            return View(record);
        }

        // GET: DomainDemo/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            var record = await _context.LocalizationRecords.FindAsync(id);
            if (record == null)
            {
                return NotFoundView();
            }

            return View(record);
        }

        // POST: DomainDemo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Text")] LocalizationRecord tRecord)
        {
            if (id != tRecord.Id)
            {
                return NotFoundView();
            }

            var record = await _context.LocalizationRecords.FindAsync(id);
            record.Text = tRecord.Text;
            TryValidateModel(record);

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DomainExists(record.Id))
                    {
                        return NotFoundView();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(record);
        }

        public IActionResult ResetCache()
        {
            _stringExtendedLocalizerFactory.ResetCache();

            return Json(true);
        }

        private bool DomainExists(long id)
        {
            return _context.LocalizationRecords.Any(e => e.Id == id);
        }
    }
}
