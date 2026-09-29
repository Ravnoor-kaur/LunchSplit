Software Quality I: Testing Tools and Methodologies - Assignment_1

## LunchSplit

LunchSplit is a restaurant bill-splitting application developed for CSCN72010 Software Quality I. The project focuses on automated unit testing, Test-Driven Development (TDD), validation, rounding, penny reconciliation, and proportional bill splitting.

## Student

**Name:** Ravnoor Kaur

## Technologies

- C#
- .NET 8
- MSTest
- Visual Studio
- Git and GitHub

## Project Purpose

The purpose of Assignment 1 is to develop the core domain logic for splitting a restaurant bill among multiple attendees while applying tax, tips, validation rules, and rounding.

The project was developed using a Test-Driven Development approach where unit tests were used to define and verify expected behavior for the core functionality.

## Main Components

### Models

- **Bill** – Stores subtotal, tax, tip mode, and tip input.
- **Attendee** – Stores attendee name, weight, and inclusion status.
- **Share** – Stores an attendee name and their calculated share.
- **TipMode** – Defines None, Percent, and Fixed tip options.
- **RoundingMode** – Defines the supported rounding behaviors.

### Engines

- **Splitter** – Calculates tips and distributes the bill among attendees.
- **Rounder** – Applies the selected rounding mode to calculated shares and handles rounding reconciliation.

### Helpers

- **BillValidator** – Validates bill and attendee information.
- **ValidationResult** – Stores the validation result and error message.

### Output

- **ReceiptFormatter** – Creates a formatted receipt containing the student name, date, creation timestamp, and individual share amounts.

## Tip Calculation

The `Splitter` supports three tip modes:

- **None** – No tip is added.
- **Percent** – Calculates the tip as a percentage of the subtotal.
- **Fixed** – Adds a specified fixed tip amount.

Invalid values such as a negative subtotal or negative fixed tip are rejected.

## Bill Splitting

The bill is distributed among included attendees according to their weights.

For example, if a $90 bill is split between two included attendees with weights of 2 and 1:

- Attendee 1 receives $60
- Attendee 2 receives $30

Excluded attendees receive a share of $0.

The implementation also prevents calculations when all attendees are excluded or when the total included weight is zero.

## Rounding

The project supports different rounding modes, including:

- No rounding
- Banker's rounding
- Round up
- Round down

Penny reconciliation is used for uneven splits so that the rounded shares can add up to the required bill total.

For example:

```text
$10.00 / 3 attendees

$3.34
$3.33
$3.33

Total = $10.00