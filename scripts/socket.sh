#!/bin/sh

LETTERBOOK_PORT=5127
SOCKET_DIR="$(dirname "$0")/../sockets"

socat "unix-listen:$SOCKET_DIR/host.sock,mode=777,fork" "tcp:127.0.0.1:$LETTERBOOK_PORT"
