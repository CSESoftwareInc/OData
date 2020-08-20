# CSESoftware.OData

Library to convert Open Data REST API queries into queries for CSESoftware.Repository.

---

## Example use in an API Controller
```C#
private readonly IODataRepository _openDataRepository;

public TimesheetController(IODataRepository openDataRepository)
{
	_openDataRepository = openDataRepository;
}

[HttpGet]
public async Task<IActionResult> Query([FromQuery]ODataFilter queryOptions)
{
	// Calls CSESoftware.Repository with parameters from ODataFilter 
	var data = await _openDataRepository.GetEntities<Timesheet>(queryOptions);

	// Todo do your mapping to view model here

	// Start building your response 
	var responseBuilder = new ResponseBuilder<Timesheet>().WithData(data);

	if (queryOptions.Count == true) // Add total and response count if requested
	{
		var totalCount = await _openDataRepository.GetTotalCount<Timesheet>(queryOptions); // Gets the total count without pagination 
		responseBuilder.WithCount(totalCount);
	}

	if (queryOptions.Links == true) // Add HATEOS links if requested
		responseBuilder.WithLinksForPagination(HttpContext.Current.Request.Url.AbsoluteUri, HttpContext.Request.Method, queryOptions.Skip, queryOptions.Take);


	return Ok(responseBuilder.Build());
}
```


## Accepted query string parameters and definition

| Parameter | Type   | Definition                                                   |
|:----------|:-------|:-------------------------------------------------------------|
| $top      | int    | How many entities to return (page size)                      |
| $skip     | int    | How many entities to skip (page number = ($skip / $top) + 1) |
| $filter   | string | What to filter the results by                                |
| $orderBy  | string | What column to order by first                                |
| $thenBy   | string | What column to order by second                               |
| $expand   | string | Expand these foreign relations                               |
| $count    | bool   | Should the response contain the total count of entities      |
| $links    | bool   | Should the response contain HATEOS links                     |


## Filter Operations

| Filter operation           | Example                                                                          | Explanation                                                                                                      |
|:---------------------------|:---------------------------------------------------------------------------------|:-----------------------------------------------------------------------------------------------------------------|
| Equal                      | user?$filter=createdBy eq 'KWP'                                                  | Query on User to find createdBy that equals 'KWP'                                                                |
| Less than                  | user?$filter=percent lt 100                                                      | Returns all users with percent less than 100                                                                     |
| Greater than               | user?$filter=id gt 1500 <br> user?$filter=createdDate gt 2020-04-23T03:52:37.00Z | Returns users of ID 1501 and higher <br> Can be used on dates as well.                                           |
| Greater than or equal to   | user?$filter=id ge 1500                                                          | Returns users of ID 1500 and higher                                                                              |
| Less than or equal to      | user?$filter=id le 1300                                                          | Returns users of ID 1300 and lower                                                                               |
| Different from (not equal) | user?$filter=city ne 'PEORIA'                                                    | Returns all users who's city is not "PEORIA"                                                                     |
| And                        | user?$filter=percent gt 25 and city ne 'PEORIA'                                  | Returns all users who's city is not "PEORIA" and who's percent is 26 and higher                                  |
| Or                         | user?$filter=city eq 'PEORIA' or city eq 'DENVER'                                | Returns all users with city of either "PEORIA" or "DENVER"                                                       |
| Select a range of values   | user?$filter=percent gt 40 and percent lt 70                                     | Returns all users with percent values from 40-70                                                                 |
| Contains                   | user?$filter=contains(name, 'Cameron')                                           | Returns all users who's name contains "Cameron"                                                                  |
| Any                        | user?$filter=timesheets/any() <br> user?$filter=timesheets/any(t:t/Id gt 73)     | Returns all users who have any timesheets <br> Returns all users who have a timesheet with an Id greater than 73 |
| All                        | user?$filter=timesheets/all(t:t/Id gt 73)                                        | Returns all users who's timesheets all have an Id greater than 73                                                |


## Other Operations

| Operation       | Example                                                                      | Explanation                                                                                                                             |
|:----------------|:-----------------------------------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------|
| Top (page size) | user?$top=2                                                                  | Gets the first two results from the user request                                                                                        |
| Skip            | user?$skip=4                                                                 | Get all users, except the first 4 <br> Can be combined with $top to get pages of results                                                |
| Order by        | user?$orderby=name desc                                                      | Returns all users ordered by name in descending order                                                                                   |
| Then by         | user?$orderby=city&$thenby=name                                              | Returns all users ordered by city then ordered by name                                                                                  |
| Expand          | timesheet?$expand=project <br> timesheet?$expand=project,employee/department | Includes the project object with each timesheet <br> Includes the project, employee,  and the employee's department with each timesheet |
| Count           | user?$count=true                                                             | Includes the total count entities in the response                                                                                       |
| Links           | user?$links=true                                                             | Includes HATEOS links in the response                                                                                                   |

---

CSE Software Inc. is a privately held company founded in 1990. CSE develops software, AR/VR, simulation, mobile, and web technology solutions. The company also offers live, 24x7, global help desk services in 110 languages. All CSE teams are U.S. based with experience in multiple industries, including government, military, healthcare, construction, agriculture, mining, and more. CSE Software is a certified women-owned small business. Visit us online at [csesoftware.com](csesoftware.com).