FROM node:22-bookworm-slim AS base
WORKDIR /app

RUN apt-get update \
  && apt-get install -y --no-install-recommends python3 make g++ wget \
  && rm -rf /var/lib/apt/lists/*

COPY package.json ./
RUN npm install --omit=dev

COPY src ./src
RUN mkdir -p /data/uploads

ENV NODE_ENV=production
ENV PORT=4000
ENV DB_PATH=/data/erp.db
ENV UPLOAD_DIR=/data/uploads

EXPOSE 4000
CMD ["node", "src/index.js"]
