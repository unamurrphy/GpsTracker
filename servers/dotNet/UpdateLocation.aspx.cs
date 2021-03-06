using System;
using System.IO;
using System.Net;
using System.Text;
using System.Data.SqlClient;

public partial class UpdateLocation : System.Web.UI.Page {

    protected void Page_Load(object sender, EventArgs e) {

        // http://localhost/gpstracker/UpdateLocation.aspx?longitude=-122.0214996&latitude=47.4758847&extrainfo=0&username=momo&distance=0.012262854&date=2014-09-16%2B17%253A49%253A57&direction=0&accuracy=65&phonenumber=867-5309&eventtype=android&sessionid=0a6dfd74-df4d-466e-b1b8-23234ef57512&speed=0&locationmethod=fused

        string latitude = Request.QueryString["latitude"];
        string longitude = Request.QueryString["longitude"];
        string sessionID = Request.QueryString["sessionid"];
        string userName = Request.QueryString["username"];

        Decimal latDecimal;
        bool result = Decimal.TryParse(latitude, out latDecimal);
        if (!result)
        {
            latDecimal = 0.0M;
        }

        // do a little validation
        Decimal lngDecimal;
        bool result2 = Decimal.TryParse(longitude, out lngDecimal);
        if (!result2)
        {
            lngDecimal = 0.0M;
        }

        if (latDecimal == 0.0M && lngDecimal == 0.0M)
        {
            Response.Write("-1");
            return;
        }

        if (sessionID.Trim().Length == 0)
        {
            Response.Write("-2");
            return;
        }

        if (userName.Trim().Length == 0)
        {
            Response.Write("-3");
            return;
        }

        string speed = Request.QueryString["speed"];
        string direction = Request.QueryString["direction"];
        string distance = Request.QueryString["distance"];
        string date = Server.UrlDecode(Request.QueryString["date"]);

        DateTime tempDateTime;
        bool result3 = DateTime.TryParse(date, out tempDateTime);
        if (!result3)
        {
            tempDateTime = DateTime.Now;
        }

        string locationMethod = Server.UrlDecode(Request.QueryString["locationmethod"]);
        string phoneNumber = Request.QueryString["phonenumber"];
        string accuracy = Request.QueryString["accuracy"];
        string eventType = Request.QueryString["eventtype"];
        string extraInfo = Request.QueryString["extrainfo"];

        // our helper class to update the database
        DbWriter dbw = new DbWriter();
        string returnValue = "";
        try {

            // update the database with our GPS data from the phone
            returnValue = dbw.updateDB("prcSaveGPSLocation",
                new SqlParameter("@latitude", latDecimal),
                new SqlParameter("@longitude", lngDecimal),
                new SqlParameter("@speed", speed),
                new SqlParameter("@direction", direction),
                new SqlParameter("@distance", distance),
                new SqlParameter("@date", tempDateTime),

                new SqlParameter("@locationMethod", locationMethod),
                new SqlParameter("@phoneNumber", phoneNumber),
                new SqlParameter("@userName", userName),
                new SqlParameter("@sessionID", sessionID),
                new SqlParameter("@accuracy", accuracy),
                new SqlParameter("@eventType", eventType),
                new SqlParameter("@extraInfo", extraInfo));

        }
        catch (Exception ex) {
            Response.Write(ex.Message);
        }

        Response.Write(returnValue);
    }
}
