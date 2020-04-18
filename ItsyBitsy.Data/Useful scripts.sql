select pageO.*, crossRes.DownloadMs + pageO.DownloadTime [All Content DownloadTime]
from [Page] pageO 
	cross apply 
		(select SUM(pageI.DownloadTime) DownloadMs, ParentPageId
		from [Page] pageI
		where ParentPageId = PageO.Id
		group by ParentPageId) crossRes