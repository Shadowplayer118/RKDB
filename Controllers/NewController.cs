using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RKDB.Entities;
using RKDB.ViewModel;

namespace RKDB.Controllers
{
    public class NewController : Controller
    {
        private readonly MonsalesUtangananContext _context;

        public NewController(MonsalesUtangananContext context)
        {
            _context = context;
        }

        // GET: Client
        public async Task<IActionResult> Index()
        {
            var client = (
                from clientInfo in _context.Clientinfos
                join usertype in _context.Usertypes
                on clientInfo.Usertype equals usertype.Id

                select new ClientInfoViewModel
                {
                    Id = clientInfo.Id,
                    Usertype = clientInfo.Usertype,
                    UserTypeName = usertype.Name,
                    Firstname = clientInfo.Firstname,
                    Middlename = clientInfo.Middlename,
                    Lastname = clientInfo.Lastname,
                    Address = clientInfo.Address,
                    Zipcode = clientInfo.Zipcode,
                    Birthday = clientInfo.Birthday,
                    Age = clientInfo.Age,
                    Nameoffather = clientInfo.Nameoffather,
                    Nameofmother = clientInfo.Nameofmother,
                    CivilStatus = clientInfo.CivilStatus,
                    Religion = clientInfo.Religion,
                    Occupation = clientInfo.Occupation,
                }
            ).AsAsyncEnumerable();


            if (client != null)
                return View(client);
            else
                return Problem("walay clients");

        }
        
        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.Clientinfos.FindAsync(id);

            if (clientInfo == null)
            {
                return NotFound();
            }

            // Fetch the associated UserType
            var userType = await _context.Usertypes.FindAsync(clientInfo.Usertype);
            if (userType == null)
            {
                return NotFound("User type not found for the client.");
            }

            // Fetch the loans associated with the client
            var loans = await _context.Loans.Where(l => l.ClientId == id).ToListAsync();

            // Convert each Loan entity to LoanViewModel
            var loanViewModels = loans.Select(loan => new LoanViewModel
            {
                Id = loan.Id,
                ClientId = loan.ClientId,
                Amount = loan.Amount,
                Term = loan.Term,
                Interest = loan.Interest,
                Deduction = loan.Deduction,
                Duedate = loan.Duedate,
                Payment = loan.Payment,
                Status = loan.Status,
                PaymentAmount = loan.PaymentAmount,
                Penalty = loan.Penalty,
                Total = loan.Total,
                PayableAmount = loan.PayableAmount,
                DateCreated = DateTime.Now
            }).ToList();

            // Convert the Clientinfo model to ClientInfoViewModel
            var viewModel = new ClientInfoViewModel
            {
                Id = clientInfo.Id,
                Usertype = clientInfo.Usertype,
                UserTypeName = userType.Name, // Add UserTypeName
                Firstname = clientInfo.Firstname,
                Middlename = clientInfo.Middlename,
                Lastname = clientInfo.Lastname,
                Address = clientInfo.Address,
                Zipcode = clientInfo.Zipcode,
                Birthday = clientInfo.Birthday,
                Age = clientInfo.Age,
                Nameoffather = clientInfo.Nameoffather,
                Nameofmother = clientInfo.Nameofmother,
                CivilStatus = clientInfo.CivilStatus,
                Religion = clientInfo.Religion,
                Occupation = clientInfo.Occupation,
                Loans = loanViewModels // Assign the loanViewModels to the ViewModel
            };

            return View(viewModel);
        }
        
        public IActionResult SubmitPenalty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanViewModel = new LoanViewModel
            {
                ClientId = id.Value
            };

            return View(loanViewModel);
        }

        [HttpPost]
        public IActionResult SubmitPenalty(int Id, int Penalty)
        {
            _context.Database.ExecuteSqlInterpolated($"UPDATE Loan SET Penalty = {Penalty}, Status = CASE WHEN {Penalty} > 0 THEN 'Penalized' ELSE Status END WHERE Id = {Id}");

            return RedirectToAction("Profile", "New", new { Id = _context.Loans.Find(Id)?.ClientId });
        }


        [HttpPost]
        public IActionResult DeductPayment(int scheduleId, decimal paymentAmount)
        {
            try
            {
                var schedule = _context.Schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
                if (schedule == null)
                {
                    return NotFound("Schedule not found");
                }

                var loanId = schedule.LoanId;
                var loan = _context.Loans.Find(loanId);
                if (loan == null)
                {
                    return NotFound("Loan not found");
                }

                decimal remainingPayment = paymentAmount;
                var currentSchedule = schedule;

                while (remainingPayment > 0 && currentSchedule != null)
                {
                    var totalPaymentsForSchedule = _context.Transactions
                        .Where(t => t.ScheduleId == currentSchedule.ScheduleId)
                        .Sum(t => t.PaymentAmount) ?? 0;

                    decimal paymentNeeded = (currentSchedule.PaymentAmount ?? 0) - totalPaymentsForSchedule;
                    decimal paymentForCurrentSchedule = Math.Min(remainingPayment, paymentNeeded);

                    if (paymentForCurrentSchedule > 0)
                    {
                        var transaction = new Transaction
                        {
                            ScheduleId = currentSchedule.ScheduleId,
                            PaymentAmount = paymentForCurrentSchedule,
                            TransactionDate = DateTime.Now
                        };

                        _context.Transactions.Add(transaction);
                        remainingPayment -= paymentForCurrentSchedule;
                    }

                    // Update status based on the total payment amount
                    totalPaymentsForSchedule += paymentForCurrentSchedule;
                    if (totalPaymentsForSchedule == currentSchedule.PaymentAmount)
                    {
                        currentSchedule.Status = "Paid";
                    }
                    else if (totalPaymentsForSchedule > 0)
                    {
                        currentSchedule.Status = "Partial";
                    }

                    var dbSchedule = _context.Schedules.FirstOrDefault(s => s.ScheduleId == currentSchedule.ScheduleId);
                    if (dbSchedule != null)
                    {
                        dbSchedule.Status = currentSchedule.Status;
                    }

                    // Attempt to find the next schedule
                    var nextSchedule = _context.Schedules
                        .Where(s => s.LoanId == loanId && s.ScheduleDate > currentSchedule.ScheduleDate)
                        .OrderBy(s => s.ScheduleDate)
                        .FirstOrDefault();

                    if (nextSchedule != null)
                    {
                        currentSchedule = nextSchedule;
                    }
                    else
                    {
                        // If no next schedule, find the previous schedule
                        var previousSchedule = _context.Schedules
                            .Where(s => s.LoanId == loanId && s.ScheduleDate < currentSchedule.ScheduleDate)
                            .OrderByDescending(s => s.ScheduleDate)
                            .FirstOrDefault();

                        // Continue deducting from previous schedules until remainingPayment is exhausted or no more previous schedules are available
                        while (remainingPayment > 0 && previousSchedule != null)
                        {
                            totalPaymentsForSchedule = _context.Transactions
                                .Where(t => t.ScheduleId == previousSchedule.ScheduleId)
                                .Sum(t => t.PaymentAmount) ?? 0;

                            paymentNeeded = (previousSchedule.PaymentAmount ?? 0) - totalPaymentsForSchedule;
                            paymentForCurrentSchedule = Math.Min(remainingPayment, paymentNeeded);

                            if (paymentForCurrentSchedule > 0)
                            {
                                var transaction = new Transaction
                                {
                                    ScheduleId = previousSchedule.ScheduleId,
                                    PaymentAmount = paymentForCurrentSchedule,
                                    TransactionDate = DateTime.Now
                                };

                                _context.Transactions.Add(transaction);
                                remainingPayment -= paymentForCurrentSchedule;
                            }

                            // Update status based on the total payment amount
                            totalPaymentsForSchedule += paymentForCurrentSchedule;
                            if (totalPaymentsForSchedule == previousSchedule.PaymentAmount)
                            {
                                previousSchedule.Status = "Paid";
                            }
                            else if (totalPaymentsForSchedule > 0)
                            {
                                previousSchedule.Status = "Partial";
                            }

                            dbSchedule = _context.Schedules.FirstOrDefault(s => s.ScheduleId == previousSchedule.ScheduleId);
                            if (dbSchedule != null)
                            {
                                dbSchedule.Status = previousSchedule.Status;
                            }

                            previousSchedule = _context.Schedules
                                .Where(s => s.LoanId == loanId && s.ScheduleDate < previousSchedule.ScheduleDate)
                                .OrderByDescending(s => s.ScheduleDate)
                                .FirstOrDefault();
                        }

                        currentSchedule = null;
                    }
                }

                // Update the loan's PayableAmount
                loan.PayableAmount -= paymentAmount;

                // Check if the loan is fully paid
                if (loan.PayableAmount <= 0)
                {
                    loan.Status = "Paid";
                }
                // Save changes to the database
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deducting payment: {ex.Message}");
            }
        }

       [HttpGet]
        public IActionResult GetLoanSchedules(int loanId)
        {
            try
            {
                // Fetch all schedules for the loan
                var schedules = _context.Schedules
                    .Where(s => s.LoanId == loanId)
                    .OrderBy(s => s.ScheduleDate)
                    .Select(s => new ScheduleViewModel
                    {
                        ScheduleId = s.ScheduleId,
                        LoanId = s.LoanId,
                        ScheduleDate = s.ScheduleDate,
                        PaymentAmount = s.PaymentAmount ?? 0,
                        Status = s.Status,
                        TotalPaidAmount = _context.Transactions
                            .Where(t => t.ScheduleId == s.ScheduleId)
                            .Sum(t => t.PaymentAmount) ?? 0,
                        AdjustedPaymentAmount = (s.PaymentAmount ?? 0) - 
                            (_context.Transactions
                                .Where(t => t.ScheduleId == s.ScheduleId)
                                .Sum(t => t.PaymentAmount) ?? 0)
                    })
                    .ToList();

                return Json(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching loan schedules: {ex.Message}");
            }
        }
        
       [HttpGet]
        public IActionResult GetScheduleReceiptDetails(int scheduleId)
        {
            try
            {
                // Fetch the schedule details
                var schedule = _context.Schedules
                    .Where(s => s.ScheduleId == scheduleId)
                    .Select(s => new
                    {
                        ScheduleId = s.ScheduleId,
                        LoanId = s.LoanId,
                        TotalAmount = s.PaymentAmount ?? 0,
                        Status = s.Status
                    })
                    .FirstOrDefault();

                if (schedule == null)
                {
                    return NotFound("Schedule not found");
                }

                // Fetch all transactions for the schedule
                var transactions = _context.Transactions
                    .Where(t => t.ScheduleId == scheduleId)
                    .OrderBy(t => t.TransactionDate) // Ensure transactions are ordered by date
                    .Select(t => new
                    {
                        TransactionId = t.TransactionId,
                        PaymentAmount = t.PaymentAmount ?? 0,
                        TransactionDate = t.TransactionDate
                    })
                    .ToList();

                if (!transactions.Any())
                {
                    return NotFound("No transactions found for this schedule");
                }

                // Get the last payment date
                var lastPaymentDate = transactions.Last().TransactionDate;

                var receiptDetails = new
                {
                    Schedule = schedule,
                    LastPaymentDate = lastPaymentDate,
                    Transactions = transactions
                };

                return Json(receiptDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching receipt details: {ex.Message}");
            }
        }




        public IActionResult AddLoan(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanViewModel = new LoanViewModel
            {
                ClientId = id.Value
            };

            return View(loanViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLoan(LoanViewModel loanViewModel)
        {
            if (ModelState.IsValid)
            {
                // Create a new Loan entity
                var loan = new Loan
                {
                    ClientId = loanViewModel.ClientId,
                    Amount = loanViewModel.Amount,
                    Term = loanViewModel.Term,
                    Interest = loanViewModel.Interest,
                    Deduction = loanViewModel.Deduction,
                    Duedate = loanViewModel.Duedate,
                    Payment = loanViewModel.Payment,
                    PaymentAmount = loanViewModel.PaymentAmount,
                    Total = loanViewModel.Total,
                    Status = "Ongoing",
                    PayableAmount = loanViewModel.Amount,
                    DateCreated = DateTime.Now
                };

                // Add the new loan to the database
                _context.Loans.Add(loan);
                await _context.SaveChangesAsync();

                // Generate schedule entries
                var scheduleEntries = new List<Schedule>();
                var currentDate = DateTime.Now.Date;

                for (int i = 0; i < loan.Term; i++)
                {
                    var scheduleEntry = new Schedule
                    {
                        LoanId = loan.Id, // Assuming Id is the primary key of the Loan entity
                        ScheduleDate = currentDate,
                        PaymentAmount = loan.PaymentAmount
                    };

                    scheduleEntries.Add(scheduleEntry);

                    // Increment the current date based on payment frequency
                    switch (loan.Payment)
                    {
                        case "Daily":
                            currentDate = currentDate.AddDays(1);
                            break;
                        case "Weekly":
                            currentDate = currentDate.AddDays(7);
                            break;
                        case "Monthly":
                            currentDate = currentDate.AddMonths(1);
                            break;
                        default:
                            break;
                    }
                }

                // Add the schedule entries to the database
                _context.Schedules.AddRange(scheduleEntries);
                await _context.SaveChangesAsync();

                // Redirect back to the client's profile using the client ID
                return RedirectToAction("Profile", "New", new { id = loanViewModel.ClientId });
            }

            // If ModelState is not valid, return the view with validation errors
            return View(loanViewModel);
        }
        public async Task<IActionResult> AddPayment(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if (loan == null)
            {
                return NotFound("Loan not found");
            }

            return Json(loan);
        }




        public IActionResult Create()
        {
            ViewData["Usertypes"] = _context.Usertypes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Clientinfo clientInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clientInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Populate ClientInfoViewModel with necessary data
            var viewModel = new ClientInfoViewModel
            {
                // Populate properties as needed
                Usertype = clientInfo.Usertype,
                Firstname = clientInfo.Firstname,
                Middlename = clientInfo.Middlename,
                Lastname = clientInfo.Lastname,
                Address = clientInfo.Address,
                Zipcode = clientInfo.Zipcode,
                Birthday = clientInfo.Birthday,
                Age = clientInfo.Age,
                Nameoffather = clientInfo.Nameoffather,
                Nameofmother = clientInfo.Nameofmother,
                CivilStatus = clientInfo.CivilStatus,
                Religion = clientInfo.Religion,
                Occupation = clientInfo.Occupation
            };

            // Pass the ViewModel to the view
            ViewData["Usertypes"] = _context.Usertypes.ToList(); 
            return View(viewModel);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || _context.Clientinfos == null)
            {
                return NotFound();
            }

            // Fetch the client info from the database
            var clientInfo = await _context.Clientinfos.FindAsync(id);

            if (clientInfo == null)
            {
                return NotFound();
            }

            // Convert the Clientinfo model to ClientInfoViewModel
            var viewModel = new ClientInfoViewModel
            {
                Id = clientInfo.Id,
                Usertype = clientInfo.Usertype,
                Firstname = clientInfo.Firstname,
                Middlename = clientInfo.Middlename,
                Lastname = clientInfo.Lastname,
                Address = clientInfo.Address,
                Zipcode = clientInfo.Zipcode,
                Birthday = clientInfo.Birthday,
                Age = clientInfo.Age,
                Nameoffather = clientInfo.Nameoffather,
                Nameofmother = clientInfo.Nameofmother,
                CivilStatus = clientInfo.CivilStatus,
                Religion = clientInfo.Religion,
                Occupation = clientInfo.Occupation
            };

            ViewData["Usertypes"] = _context.Usertypes.ToList(); 
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Usertype,Firstname,Middlename,Lastname,Address,Zipcode,Birthday,Age,Nameoffather,Nameofmother,CivilStatus,Religion,Occupation")] Clientinfo clientInfo)
        {
            if (id != clientInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientInfoExists(clientInfo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(clientInfo);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.Clientinfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clientInfo == null)
            {
                return NotFound();
            }

            _context.Clientinfos.Remove(clientInfo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        private bool ClientInfoExists(int id)
        {
            return (_context.Clientinfos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}