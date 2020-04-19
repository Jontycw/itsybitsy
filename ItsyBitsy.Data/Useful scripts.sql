select crossPAge.DlTime + outerp.DownloadTime [All Resource], outerp.*
from [Page] outerp
	outer apply(
		select sum(childPage.DownloadTime) [DlTime]
		from [Page] childPage inner join PageRelation pr on childPage.Id = pr.ChildPageId
		where pr.ParentPageId = outerp.Id and childPage.ContentType <> 4) crossPage
where SessionId = 2 and crossPage.DlTime is not null