<?php
header('Content-Type: application/json; charset=utf-8');
header('Cache-Control: no-cache');
$action = $_GET['action'] ?? 'list';
try {
    switch ($action) {
        case 'list':
            $limit = isset($_GET['limit']) ? (int)$_GET['limit'] : 100;
            if ($limit < 1) $limit = 1;
            if ($limit > 2000) $limit = 2000;
            $url = 'https://api.coincap.io/v2/assets?limit=' . $limit;
            respond_json(http_get_json($url));
            break;
        case 'detail':
            $id = trim($_GET['id'] ?? '');
            if ($id === '') throw new Exception('missing id', 400);
            $url = 'https://api.coincap.io/v2/assets/' . rawurlencode($id);
            respond_json(http_get_json($url));
            break;
        case 'history':
            $id = trim($_GET['id'] ?? '');
            if ($id === '') throw new Exception('missing id', 400);
            $days = isset($_GET['days']) ? max(1, (int)$_GET['days']) : 7;
            $endMs = (int)(microtime(true) * 1000);
            $startMs = $endMs - ($days * 24 * 60 * 60 * 1000);
            $url = 'https://api.coincap.io/v2/assets/' . rawurlencode($id) . '/history?interval=d1&start=' . $startMs . '&end=' . $endMs;
            respond_json(http_get_json($url));
            break;
        default:
            throw new Exception('Unsupported action', 400);
    }
} catch (Exception $e) {
    $code = $e->getCode() ?: 500;
    http_response_code($code);
    echo json_encode(['error' => $e->getMessage()]);
    exit;
}
function respond_json($data) {
    if ($data === null) {
        http_response_code(502);
        echo json_encode(['error' => 'Upstream API error']);
        exit;
    }
    echo json_encode($data);
    exit;
}
function http_get_json(string $url): ?array {
    $ch = curl_init($url);
    curl_setopt_array($ch, [
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_FOLLOWLOCATION => true,
        CURLOPT_CONNECTTIMEOUT => 8,
        CURLOPT_TIMEOUT => 12,
        CURLOPT_HTTPHEADER => [
            'Accept: application/json',
            'User-Agent: CryptoMania/1.0 (+student project)'
        ],
    ]);
    $out = curl_exec($ch);
    $err = curl_error($ch);
    $code = (int)curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);
    if ($err || $code >= 400 || !$out) return null;
    $json = json_decode($out, true);
    return is_array($json) ? $json : null;
}
