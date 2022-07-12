# HireHub Web Portal Code
This project is designed to be a multi-tier application. Following is the high-level description of tiers used in the application.

**HRAnalytics.Web**
This is the User Interface project of the application based out ot Microsoft.NET MVC 6. It is based on MVC (Model-View-Controller) architecture.

**HRAnalytics.WebAPI**
This project has the RESTFUL APIs which interact with the database. These APIs can be consumed by mobile devices as well.

**HRAnalytics.BL**
This project contains the business logic of the application and all the analytical model implementations (decision making techniques and linear programming)

**HRAnalytics.DAL**
This project is the data access layer which communicates with the database using stored procedures.

**HRAnalytics.Entities**
This project is a generic one which is used across the application. It mainly contains the entities (classes) that are used in API and Web project.
