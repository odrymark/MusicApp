FROM node:22.12-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

RUN mkdir -p /etc/nginx/certs && \
    apk add --no-cache openssl && \
    openssl req -x509 -newkey rsa:4096 -nodes -out /etc/nginx/certs/cert.pem \
    -keyout /etc/nginx/certs/key.pem -days 365 -subj "/CN=localhost"

EXPOSE 443
CMD ["nginx", "-g", "daemon off;"]