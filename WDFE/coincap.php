<?php
header('Content-Type: application/json; charset=utf-8');

putenv('http_proxy=');
putenv('https_proxy=');
putenv('HTTP_PROXY=');
putenv('HTTPS_PROXY=');
putenv('NO_PROXY=*');

$debug  = isset($_GET['debug']);
$action = $_GET['action'] ?? 'list';

switch ($action) {
  case 'list':
    $limit = isset($_GET['limit']) ? (int)$_GET['limit'] : 50;
    $url = 'https://api.coincap.io/v2/assets?limit='.$limit;
    $res = fetch_json($url, get_key());
    if (!$res['ok']) $res = fallback_list($limit);
    output($res, $debug);
    break;

  case 'detail':
    $id = trim($_GET['id'] ?? '');
    if ($id==='') fail(400,'missing id');
    $url = 'https://api.coincap.io/v2/assets/'.rawurlencode($id);
    $res = fetch_json($url, get_key());
    if (!$res['ok']) $res = fallback_detail($id);
    output($res, $debug);
    break;

  case 'history':
    $id = trim($_GET['id'] ?? '');
    if ($id==='') fail(400,'missing id');
    $days = isset($_GET['days']) ? max(1,(int)$_GET['days']) : 7;
    $end = (int)(microtime(true) * 1000);
    $start = $end - $days*24*60*60*1000;
    $url = 'https://api.coincap.io/v2/assets/'.rawurlencode($id).'/history?interval=d1&start='.$start.'&end='.$end;
    $res = fetch_json($url, get_key());
    if (!$res['ok']) $res = fallback_history($id, $days);
    output($res, $debug);
    break;

  default:
    fail(400,'unsupported action');
}

function fetch_json($url, $key=null){
  $ch = curl_init($url);
  $headers = ['Accept: application/json','User-Agent: CryptoMania/1.0'];
  if ($key) $headers[] = 'Authorization: Bearer '.$key;
  curl_setopt_array($ch, [
    CURLOPT_RETURNTRANSFER=>true,
    CURLOPT_FOLLOWLOCATION=>true,
    CURLOPT_CONNECTTIMEOUT=>8,
    CURLOPT_TIMEOUT=>12,
    CURLOPT_HTTPHEADER=>$headers,
    CURLOPT_PROXY=>'',
  ]);
  if (defined('CURLOPT_NOPROXY')) curl_setopt($ch, CURLOPT_NOPROXY, '*');

  $body = curl_exec($ch);
  $err  = curl_error($ch);
  $code = (int)curl_getinfo($ch, CURLINFO_HTTP_CODE);
  curl_close($ch);

  if ($code>=400 || !$body) return ['ok'=>false,'code'=>$code,'err'=>$err,'raw'=>$body];
  $json = json_decode($body, true);
  if (!is_array($json)) return ['ok'=>false,'code'=>$code,'err'=>'non-json','raw'=>$body];
  return ['ok'=>true,'code'=>$code,'json'=>$json];
}

function get_key(){
  $p = __DIR__.'/config.php';
  if (is_file($p)) { $a = require $p; if (!empty($a['coincap_api_key'])) return $a['coincap_api_key']; }
  $env = getenv('COINCAP_API_KEY'); if ($env) return $env;
  return null;
}

function output($res, $debug){
  if ($debug) { echo json_encode($res); return; }
  if ($res['ok']) { echo json_encode($res['json']); return; }
  http_response_code(502); echo json_encode(['error'=>'upstream error','code'=>$res['code']??0]); 
}

function fail($c,$m){ http_response_code($c); echo json_encode(['error'=>$m]); exit; }

/* ---------- Fallback to CoinGecko ---------- */

function fallback_list($limit){
  $u = 'https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page='.(int)$limit.'&page=1&sparkline=false&price_change_percentage=24h';
  $r = fetch_json($u, null);
  if (!$r['ok']) return $r;
  $arr = $r['json'];
  $data = [];
  foreach ($arr as $c){
    $data[] = [
      'id' => (string)$c['id'],
      'rank' => isset($c['market_cap_rank']) ? (string)$c['market_cap_rank'] : '',
      'symbol' => strtoupper($c['symbol']),
      'name' => $c['name'],
      'priceUsd' => (string)$c['current_price'],
      'marketCapUsd' => (string)$c['market_cap'],
      'volumeUsd24Hr' => (string)$c['total_volume']
    ];
  }
  return ['ok'=>true,'code'=>200,'json'=>['data'=>$data]];
}

function fallback_detail($id){
  $u = 'https://api.coingecko.com/api/v3/coins/'.rawurlencode($id).'?localization=false&tickers=false&market_data=true&community_data=false&developer_data=false&sparkline=false';
  $r = fetch_json($u, null);
  if (!$r['ok']) return $r;
  $c = $r['json'];
  $m = $c['market_data'];
  $data = [
    'id' => (string)$c['id'],
    'symbol' => strtoupper($c['symbol']),
    'name' => $c['name'],
    'priceUsd' => (string)$m['current_price']['usd'],
    'marketCapUsd' => (string)$m['market_cap']['usd'],
    'volumeUsd24Hr' => (string)$m['total_volume']['usd'],
    'supply' => (string)$m['circulating_supply']
  ];
  return ['ok'=>true,'code'=>200,'json'=>['data'=>$data]];
}

function fallback_history($id, $days){
  $u = 'https://api.coingecko.com/api/v3/coins/'.rawurlencode($id).'/market_chart?vs_currency=usd&days='.(int)$days.'&interval=daily';
  $r = fetch_json($u, null);
  if (!$r['ok']) return $r;
  $pairs = $r['json']['prices']; 
  $data = [];
  foreach ($pairs as $p){
    $ts = (int)$p[0]; $price = (float)$p[1];
    $data[] = ['date'=>gmdate('c', $ts/1000), 'priceUsd'=>(string)$price];
  }
  return ['ok'=>true,'code'=>200,'json'=>['data'=>$data]];
}
