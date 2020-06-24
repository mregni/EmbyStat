export const CommunityRatingFilter = [{
  text: 'None',
  value: ''
}, {
  text: '0 - 1',
  value: ['communityRating', '<', 1]
}, {
  text: '1 - 2',
  value: [
    ['communityRating', '>=', 1],
    ['communityRating', '<', 2]
  ]
}, {
  text: '2 - 3',
  value: [
    ['communityRating', '>=', 2],
    ['communityRating', '<', 3]
  ]
}, {
  text: '3 - 4',
  value: [
    ['communityRating', '>=', 3],
    ['communityRating', '<', 4]
  ]
}, {
  text: '4 - 5',
  value: [
    ['communityRating', '>=', 4],
    ['communityRating', '<', 5]
  ]
}, {
  text: '5 - 6',
  value: [
    ['communityRating', '>=', 5],
    ['communityRating', '<', 6]
  ]
}, {
  text: '6 - 7',
  value: [
    ['communityRating', '>=', 6],
    ['communityRating', '<', 7]
  ]
}, {
  text: '7 - 8',
  value: [
    ['communityRating', '>=', 7],
    ['communityRating', '<', 8]
  ]
}, {
  text: '8 - 9',
  value: [
    ['communityRating', '>=', 8],
    ['communityRating', '<', 9]
  ]
}, {
  text: '9 - 10',
  value: ['communityRating', '>=', 9]
}];
