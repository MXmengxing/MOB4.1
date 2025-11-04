<?php
header('Content-Type: application/json; charset=utf-8');

$q = $_GET['q'] ?? 'cryptocurrency OR bitcoin OR ethereum OR blockchain';
$pageSize = isset($_GET['pageSize']) ? (int)$_GET['pageSize'] : 20;

$cfg = is_file(__DIR__.'/config_news.php') ? require __DIR__.'/config_news.php' : [];
$key = $cfg['news_api_key'] ?? getenv('NEWS_API_KEY');

if (!$key) { http_response_code(400); echo json_encode(['error'=>'missing api key']); exit; }

$url = 'https://newsapi.org/v2/everything?' . http_build_query([
  'q' => $q,
  'language' => 'en',
  'sortBy' => 'publishedAt',
  'pageSize' => $pageSize,
]);

$ch = curl_init($url);
curl_setopt_array($ch, [
  CURLOPT_RETURNTRANSFER => true,
  CURLOPT_TIMEOUT => 12,
  CURLOPT_HTTPHEADER => ['X-Api-Key: '.$key, 'User-Agent: CryptoMania/1.0'],
  CURLOPT_PROXY => '',
]);
if (defined('CURLOPT_NOPROXY')) curl_setopt($ch, CURLOPT_NOPROXY, '*');

$body = curl_exec($ch);
$code = (int)curl_getinfo($ch, CURLINFO_HTTP_CODE);
$err  = curl_error($ch);
curl_close($ch);

if ($code>=400 || !$body) { http_response_code(502); echo json_encode(['error'=>'upstream','code'=>$code,'err'=>$err]); exit; }
echo $body;
