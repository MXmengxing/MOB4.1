var API = "./coincap.php";

function money(x){
  var n = Number(x); if(!isFinite(n)) return "-";
  if(n>1) return n.toLocaleString(undefined,{style:"currency",currency:"USD",maximumFractionDigits:2});
  if(n>0.01) return "$"+n.toLocaleString(undefined,{maximumFractionDigits:4});
  return "$"+n.toExponential(2);
}
function compactNum(x){
  var n = Number(x); if(!isFinite(n)) return "-";
  if(n>=1e12) return "$"+(n/1e12).toFixed(2)+"T";
  if(n>=1e9)  return "$"+(n/1e9).toFixed(2)+"B";
  if(n>=1e6)  return "$"+(n/1e6).toFixed(2)+"M";
  if(n>=1e3)  return "$"+(n/1e3).toFixed(2)+"K";
  return "$"+n.toFixed(2);
}

function loadList(){
  if(!$("#all-characters-table").length) return;
  $.getJSON(API,{action:"list",limit:50})
   .done(function(resp){
     var raw = (resp && resp.data) ? resp.data : [];
     var mapped = $.map(raw, function(c){
       return {
         id:c.id, symbol:c.symbol, name:c.name,
         pricePretty: money(c.priceUsd),
         marketCapPretty: compactNum(c.marketCapUsd),
         volume24hPretty: compactNum(c.volumeUsd24Hr)
       };
     });
     var tpl = $("#js-pokemon-template").html();
     var html = Mustache.render(tpl,{data:mapped});
     $("#all-characters-table tbody").html(html);
   })
   .fail(function(x){ alert("API error: "+x.status); });
}

var coinChart = null;
function openInfo(id, symbol){
  var sym = (symbol||"").toLowerCase();
  $("#coinModalIcon").attr("src","https://assets.coincap.io/assets/icons/"+sym+"@2x.png").attr("alt",symbol||"");
  $.getJSON(API,{action:"detail",id:id}).done(function(res){
    var d = (res && res.data) ? res.data : null;
    $("#coinModalLabel").text(d ? (d.name+" ("+d.symbol+")") : (symbol||""));
  });
  $.getJSON(API,{action:"history",id:id,days:7}).done(function(res){
    var arr = (res && res.data) ? res.data : [];
    var last = arr.slice(-7);
    var labels = $.map(last, function(p){ return p.date ? p.date.split("T")[0] : ""; });
    var data = $.map(last, function(p){ return Number(p.priceUsd||0); });
    var ctx = document.getElementById("coin-history-chart").getContext("2d");
    if (coinChart) coinChart.destroy();
    coinChart = new Chart(ctx,{
      type:"line",
      data:{ labels:labels, datasets:[{ label:"Price (USD)", data:data, pointRadius:0, tension:0.3 }] },
      options:{ responsive:true, scales:{ x:{ title:{display:true,text:"Date"} }, y:{ title:{display:true,text:"Price"} } } }
    });
  });
}

function getCoinInfo(btn){
  var cryptoName = $(btn).closest("tr").find(".crypto-name").text().trim();
  var cryptoPrice = $(btn).closest("tr").find(".crypto-price").text().replace(/[^0-9.\-eE]/g,"").trim();
  var tpl = $("#cryptofolio-modal-template").html();
  var html = Mustache.render(tpl,{coinName:cryptoName,coinPrice:cryptoPrice});
  $("#modal-content-cryptofolio").html(html);
  $("#amount-coins").val("");
  $("#total-value").text("0");
  $("#amount-coins").on("input", function(){
    var p = parseFloat($("#coin-price").text()) || 0;
    var a = parseFloat($("#amount-coins").val()) || 0;
    $("#total-value").text((p*a).toFixed(8));
  });
}

function addCoin(){
  var coinName = $("#coin-name").text().trim();
  var coinPrice = parseFloat($("#coin-price").text()) || 0;
  var amountCoins = parseFloat($("#amount-coins").val()) || 0;
  var totalValue = parseFloat($("#total-value").text()) || 0;
  if (!coinName || coinPrice<=0 || amountCoins<=0) return;
  $.ajax({
    type:"POST",
    url:"includes/add_coins_db.php",
    data:{ coin_name:coinName, coin_price:coinPrice, amount_coins:amountCoins, total_value:totalValue },
    success:function(){
      var el = document.getElementById("exampleModal");
      if (el) { var modal = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el); modal.hide(); }
      getAllCoinsPortfolio();
    }
  });
}

function renderPortfolio(rows){
  if (!$("#crypto-folio-table").length) return;
  var tpl = $("#coins-cryptofolio-template").html();
  var html = Mustache.render(tpl, rows);
  $("#crypto-folio-table tbody").html(html);
  var sum = 0;
  for (var i=0;i<rows.length;i++){ sum += parseFloat(rows[i].totalValue || 0); }
  $("#total-value").text(sum.toFixed(8));
}

function getAllCoinsPortfolio(){
  if (!$("#crypto-folio-table").length) return;
  $.ajax({
    type:"GET",
    url:"includes/get_coins_db.php",
    dataType:"json",
    success:function(data){ renderPortfolio(data || []); }
  });
}

function saveCoin(btn){
  var id = $(btn).val();
  var amount = $(btn).closest("tr").find(".amount-input").val();
  $.ajax({ type:"POST", url:"includes/update_coin_db.php", data:{id:id,amount:amount}, success:function(){ getAllCoinsPortfolio(); } });
}

function deleteCoin(btn){
  var id = $(btn).val();
  $.ajax({ type:"POST", url:"includes/delete_coin_db.php", data:{id:id}, success:function(){ getAllCoinsPortfolio(); } });
}

$(document).ready(function(){
  loadList();
  $("#all-characters-table").on("click",".coin-info-btn", function(){ var id=$(this).data("id"); var sym=$(this).data("symbol"); openInfo(id,sym); });
  $(document).on("click",".btn-open-modal-cryptofolio", function(){ getCoinInfo(this); });
  $(document).on("click","#js-add-coin-btn", function(){ addCoin(); });
  getAllCoinsPortfolio();
  $(document).on("click",".save-coin-btn", function(){ saveCoin(this); });
  $(document).on("click",".delete-coin-btn", function(){ deleteCoin(this); });
});
