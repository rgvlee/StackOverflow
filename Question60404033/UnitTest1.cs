using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Question60404033
{
    public class Room
    {
        public DateTime RoomBooked { get; set; }

        public List<RoomSchedule> Schedule { get; set; }
    }

    public class RoomSchedule
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator(RoomScheduleValidator roomScheduleValidator)
        {
            RuleFor(o => o.RoomBooked)
                .NotEmpty().WithMessage("Booking can not be empty.");

            //RuleFor(o => o.Schedule).Must(schedule =>
            //    {
            //        if (schedule == null || !schedule.Any())
            //        {
            //            return true;
            //        }

            //        return schedule.All(item => !schedule.Where(x => !ReferenceEquals(item, x)).Any(x => x.Start < item.End && x.End > item.Start));
            //    })
            //    .WithMessage("Schedule can not have overlapping times.");

            RuleFor(o => o.Schedule).Custom((schedule, context) =>
            {
                if (schedule == null || !schedule.Any())
                {
                    return;
                }

                foreach (var item in schedule)
                {
                    var scheduleOverlapsAnotherSchedule = schedule.Where(x => !ReferenceEquals(item, x)).Any(x => x.Start < item.End && x.End > item.Start);
                    if (scheduleOverlapsAnotherSchedule)
                    {
                        context.AddFailure($"Schedule {item.Start.ToShortTimeString()}-{item.End.ToShortTimeString()} overlaps another schedule.");
                    }
                }
            });

            RuleForEach(x => x.Schedule)
                .SetValidator(roomScheduleValidator);
        }
    }

    public class RoomScheduleValidator : AbstractValidator<RoomSchedule>
    {
        public RoomScheduleValidator()
        {
            RuleFor(o => o.Start)
                .NotEmpty().WithMessage("Start time required.")
                .NotEqual(m => m.End).WithMessage("Start time can not be the same as the end time.");

            RuleFor(m => m.End)
                .NotEmpty().WithMessage("End time required.")
                .GreaterThan(m => m.Start)
                .WithMessage("End time can not be before start time.");
        }
    }

    public class RoomScheduleValidatorTests
    {
        [Test]
        public void Start_NotSet_HasErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var schedule = new RoomSchedule { End = DateTime.Parse("2020-03-01 18:30") };

            rsv.ShouldHaveValidationErrorFor(x => x.Start, schedule).WithErrorMessage("Start time required.");
        }

        [Test]
        public void Start_LessThanEnd_HasNoErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var schedule = new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30"), End = DateTime.Parse("2020-03-01 18:30") };

            rsv.ShouldNotHaveValidationErrorFor(x => x.Start, schedule);
        }

        [Test]
        public void Start_EqualToEnd_HasErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var schedule = new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30"), End = DateTime.Parse("2020-03-01 13:30") };

            rsv.ShouldHaveValidationErrorFor(x => x.Start, schedule).WithErrorMessage("Start time can not be the same as the end time.");
        }

        [Test]
        public void End_NotSet_HasErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var schedule = new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30") };

            rsv.ShouldHaveValidationErrorFor(x => x.End, schedule).WithErrorMessage("End time required.");
        }

        [Test]
        public void End_GreaterThanStart_HasNoErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var schedule = new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30"), End = DateTime.Parse("2020-03-01 18:30") };

            rsv.ShouldNotHaveValidationErrorFor(x => x.End, schedule);
        }

        [Test]
        public void End_LessThanStart_HasErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var schedule = new RoomSchedule { Start = DateTime.Parse("2020-03-01 18:30"), End = DateTime.Parse("2020-03-01 13:30") };

            rsv.ShouldHaveValidationErrorFor(x => x.End, schedule).WithErrorMessage("End time can not be before start time.");
        }
    }

    public class RoomValidatorTests
    {
        [Test]
        public void RoomBooked_NotSet_HasErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var rv = new RoomValidator(rsv);

            var room = new Room();

            rv.ShouldHaveValidationErrorFor(x => x.RoomBooked, room).WithErrorMessage("Booking can not be empty.");
        }

        [Test]
        public void Schedule_HasChildValidator()
        {
            var rsv = new RoomScheduleValidator();
            var rv = new RoomValidator(rsv);

            rv.ShouldHaveChildValidator(x => x.Schedule, typeof(RoomScheduleValidator));
        }

        [Test]
        public void Room_TwoSchedulesWithTimesThatDoNotOverlap_HasNoErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var rv = new RoomValidator(rsv);

            var schedule = new List<RoomSchedule> {
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 07:30"), End = DateTime.Parse("2020-03-01 13:00") },
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30"), End = DateTime.Parse("2020-03-01 18:30") }
            };

            var room = new Room { RoomBooked = DateTime.Parse("2020-03-01"), Schedule = schedule };

            var validationResult = rv.TestValidate(room);

            Assert.That(validationResult.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void Room_TwoSchedulesWithTimesThatDoOverlap_HasErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var rv = new RoomValidator(rsv);

            var schedule = new List<RoomSchedule> {
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 07:30"), End = DateTime.Parse("2020-03-01 14:00") },
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30"), End = DateTime.Parse("2020-03-01 18:30") }
            };

            var room = new Room { RoomBooked = DateTime.Parse("2020-03-01"), Schedule = schedule };

            var validationResult = rv.TestValidate(room);

            //Assert.That(validationResult.Errors.Count, Is.EqualTo(1));
            //validationResult.ShouldHaveValidationErrorFor(x => x.Schedule)
            //    .WithErrorMessage("Schedule can not have overlapping times.");

            Assert.That(validationResult.Errors.Count, Is.EqualTo(2));
            validationResult.ShouldHaveValidationErrorFor(x => x.Schedule)
                .WithErrorMessage("Schedule 7:30 AM-2:00 PM overlaps another schedule.")
                .WithErrorMessage("Schedule 1:30 PM-6:30 PM overlaps another schedule.");
        }

        [Test]
        public void Room_TwoSchedulesWithTimesThatAreContiguous_HasNoErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var rv = new RoomValidator(rsv);

            var schedule = new List<RoomSchedule> {
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 07:30"), End = DateTime.Parse("2020-03-01 13:30") },
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30"), End = DateTime.Parse("2020-03-01 18:30") }
            };

            var room = new Room { RoomBooked = DateTime.Parse("2020-03-01"), Schedule = schedule };

            var validationResult = rv.TestValidate(room);

            Assert.That(validationResult.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void Room_FourSchedulesWithTimesThatDoOverlap_HasErrorMessage()
        {
            var rsv = new RoomScheduleValidator();
            var rv = new RoomValidator(rsv);

            var schedule = new List<RoomSchedule> {
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 04:30"), End = DateTime.Parse("2020-03-01 08:30") },
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 07:30"), End = DateTime.Parse("2020-03-01 14:00") },
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 13:30"), End = DateTime.Parse("2020-03-01 18:30") },
                new RoomSchedule { Start = DateTime.Parse("2020-03-01 17:30"), End = DateTime.Parse("2020-03-01 21:00") }
            };

            var room = new Room { RoomBooked = DateTime.Parse("2020-03-01"), Schedule = schedule };

            var validationResult = rv.TestValidate(room);

            //Assert.That(validationResult.Errors.Count, Is.EqualTo(1));
            //validationResult.ShouldHaveValidationErrorFor(x => x.Schedule)
            //    .WithErrorMessage("Schedule can not have overlapping times.");

            Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
            validationResult.ShouldHaveValidationErrorFor(x => x.Schedule)
                .WithErrorMessage("Schedule 4:30 AM-8:30 AM overlaps another schedule.")
                .WithErrorMessage("Schedule 7:30 AM-2:00 PM overlaps another schedule.")
                .WithErrorMessage("Schedule 1:30 PM-6:30 PM overlaps another schedule.")
                .WithErrorMessage("Schedule 5:30 PM-9:00 PM overlaps another schedule.");
        }
    }
}